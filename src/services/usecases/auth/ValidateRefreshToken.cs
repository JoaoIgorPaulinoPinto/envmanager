using envmanager.src.data.service.interfaces;
using envmanager.src.data.utils;
using envmanager.src.services.interfaces.auth;
using static envmanager.src.data.service.dtos.AuthDtos;

namespace envmanager.src.services.usecases.auth
{
    public class ValidateRefreshToken : IValidateRefreshToken
    {
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        private readonly IAuthRepository _authRepository;
        private readonly ITokenFactory _tokenFactory;

        public ValidateRefreshToken(IAuthRepository authRepository, ITokenFactory tokenFactory)
        {
            _authRepository = authRepository;
            _tokenFactory = tokenFactory;
        }

        public async Task<SessionTokens> Execute(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is missing.");

            var refreshTokenHash = _tokenFactory.HashRefreshToken(refreshToken);
            var user = await _authRepository.GetUserByRefreshToken(refreshTokenHash);

            if (user == null)
                throw new UnauthorizedAccessException("Refresh token not found or invalid.");

            if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Session has expired. Please login again.");

            var newRefreshToken = _tokenFactory.GenerateRefreshToken();
            var newRefreshTokenHash = _tokenFactory.HashRefreshToken(newRefreshToken);
            var refreshTokenExpiry = DateTime.UtcNow.Add(RefreshTokenLifetime);

            var updated = await _authRepository.UpdateRefreshToken(user.Id, newRefreshTokenHash, refreshTokenExpiry);
            if (!updated)
                throw new InvalidOperationException("Unable to rotate refresh token.");

            var accessToken = _tokenFactory.CreateUserToken(user);

            return new SessionTokens(accessToken, newRefreshToken, refreshTokenExpiry);
        }
    }
}
