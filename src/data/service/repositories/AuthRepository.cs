using envmanager.src.data.infra.db;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using MongoDB.Driver;

namespace envmanager.src.data.service.repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _appDbContext;

        public AuthRepository(AppDbContext db)
        {
            _appDbContext = db;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _appDbContext.Users
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByRefreshToken(string refreshToken)
        {
            return await _appDbContext.Users
                .Find(u => u.RefreshToken == refreshToken)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateRefreshToken(string userId, string refreshToken, DateTime refreshTokenExpiry)
        {
            var update = Builders<User>.Update
                .Set(u => u.RefreshToken, refreshToken)
                .Set(u => u.RefreshTokenExpiry, refreshTokenExpiry);

            var result = await _appDbContext.Users.UpdateOneAsync(u => u.Id == userId, update);
            return result.ModifiedCount > 0;
        }
    }
}
