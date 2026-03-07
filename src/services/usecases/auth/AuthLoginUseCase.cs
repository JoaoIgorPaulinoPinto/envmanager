using envmanager.src.data.service.interfaces;
using envmanager.src.data.utils;
using envmanager.src.services.interfaces.auth;
using static envmanager.src.data.service.dtos.AuthDtos;

namespace envmanager.src.services.usecases.auth
{
    public class AuthLoginUseCase : IAuthLoginUseCase
    {
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        private readonly IAuthRepository _authRepository;
        private readonly SecurityService _securityService;
        private readonly ITokenFactory _tokenFactory;

        public AuthLoginUseCase(IAuthRepository authRepository, SecurityService securityService, ITokenFactory tokenFactory)
        {
            _authRepository = authRepository;
            _securityService = securityService;
            _tokenFactory = tokenFactory;
        }

        public async Task<SessionTokens> Execute(LoginRequest loginRequest)
        {
            if (loginRequest == null)
                throw new ArgumentException("Login details not provided.");

            if (string.IsNullOrWhiteSpace(loginRequest.email) || string.IsNullOrWhiteSpace(loginRequest.password))
                throw new ArgumentException("Email and password are required.");

            var user = await _authRepository.GetUserByEmail(loginRequest.email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var isPasswordValid = _securityService.VerifyPassword(user.Password, loginRequest.password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var refreshToken = _tokenFactory.GenerateRefreshToken();
            var refreshTokenHash = _tokenFactory.HashRefreshToken(refreshToken);
            var refreshTokenExpiry = DateTime.UtcNow.Add(RefreshTokenLifetime);

            var updated = await _authRepository.UpdateRefreshToken(user.Id, refreshTokenHash, refreshTokenExpiry);
            if (!updated)
                throw new InvalidOperationException("Unable to persist refresh token.");

            var accessToken = _tokenFactory.CreateUserToken(user);

            return new SessionTokens(accessToken, refreshToken, refreshTokenExpiry);
        }
    }
}
