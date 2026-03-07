using envmanager.src.data.service.schemes;

namespace envmanager.src.data.service.interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByRefreshToken(string refreshTokenHash);
        Task<bool> UpdateRefreshToken(string userId, string refreshTokenHash, DateTime refreshTokenExpiry);
        Task<bool> RevokeRefreshToken(string userId);
    }
}
