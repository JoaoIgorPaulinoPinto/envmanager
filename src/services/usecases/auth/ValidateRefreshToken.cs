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
        public Task<string> Execute(string refreshToken)
        {
           return _authRepository.ValidateRefreshToken(refreshToken);
        }
    }
}
