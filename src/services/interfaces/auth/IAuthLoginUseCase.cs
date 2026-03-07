using static envmanager.src.data.service.dtos.AuthDtos;

namespace envmanager.src.services.interfaces.auth
{
    public interface IAuthLoginUseCase
    {
        Task<SessionTokens> Execute(LoginRequest loginRequest);
    }
}
