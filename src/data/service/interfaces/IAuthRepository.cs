using envmanager.src.data.service.schemes;
using static envmanager.src.data.service.dtos.AuthDtos;

namespace envmanager.src.data.service.interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByRefreshToken(string refreshToken);
        Task<bool> UpdateRefreshToken(string userId, string refreshToken, DateTime refreshTokenExpiry);
    }
}
