using envmanager.src.data.interfaces;
using envmanager.src.services.interfaces.auth;

namespace envmanager.src.services.usecases.auth
{
    public class ValidateRefreshToken : IValidateRefreshToken
    {
        private readonly IAuthRepository _authRepository;

        public ValidateRefreshToken(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<string> Execute(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token is missing.");
            }

            return await _authRepository.ValidateRefreshToken(refreshToken);
        }
    }
}