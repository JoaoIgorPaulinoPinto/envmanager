using static envmanager.src.data.service.dtos.AuthDtos;

namespace envmanager.src.services.interfaces.auth
{
    public interface IValidateRefreshToken
    {
        Task<SessionTokens> Execute(string refreshToken);
    }
}
