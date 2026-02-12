
using static envmanager.src.data.dtos.AuthDtos;

namespace envmanager.src.data.interfaces
{
    public interface IAuthRepository
    {
        public Task<string> Login(LoginRequest loginRequest, string refreshToken);
        public Task<string> ValidateRefreshToken(string refreshToken);
    }
}
