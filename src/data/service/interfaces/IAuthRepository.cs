using static envmanager.src.data.service.dtos.AuthDtos;

namespace envmanager.src.data.service.interfaces
{
    public interface IAuthRepository
    {
        public Task<string> Login(LoginRequest loginRequest, string refreshToken);
        public Task<string> ValidateRefreshToken(string refreshToken);
    }
}
