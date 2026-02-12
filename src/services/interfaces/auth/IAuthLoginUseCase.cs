using static envmanager.src.data.dtos.AuthDtos;

namespace envmanager.src.services.interfaces.auth
{
    public interface IAuthLoginUseCase
    {
        public Task<string> Execute(LoginRequest loginRequest,string refreshToken);
    }
}
