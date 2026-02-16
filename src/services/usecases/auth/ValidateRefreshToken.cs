using envmanager.src.data.service.interfaces;
using envmanager.src.data.utils;
using envmanager.src.services.interfaces.auth;

namespace envmanager.src.services.usecases.auth
{
    public class ValidateRefreshToken : IValidateRefreshToken
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenFactory _tokenFactory;

        public ValidateRefreshToken(IAuthRepository authRepository, ITokenFactory tokenFactory)
        {
            _authRepository = authRepository;
            _tokenFactory = tokenFactory;
        }

        public async Task<string> Execute(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token is missing.");
            }

            var user = await _authRepository.GetUserByRefreshToken(refreshToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Refresh token not found or invalid.");
            }

            if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Session has expired. Please login again.");
            }

            return _tokenFactory.CreateUserToken(user);
        }
    }
}
