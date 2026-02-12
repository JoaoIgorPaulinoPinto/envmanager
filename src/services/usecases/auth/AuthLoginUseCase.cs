using envmanager.src.data.interfaces;
using envmanager.src.services.interfaces.auth;
using static envmanager.src.data.dtos.AuthDtos;

namespace envmanager.src.services.usecases.auth
{
    public class AuthLoginUseCase : IAuthLoginUseCase
    {
        private readonly IAuthRepository _authRepo; 
        public AuthLoginUseCase(IAuthRepository authRepo) { _authRepo = authRepo; }

        public Task<string> Execute(LoginRequest loginRequest)
        {
            try
            {
                return _authRepo.Login(loginRequest);
            }
            catch {
                throw new Exception("Error on login");
            }
        }
    }
}
