using envmanager.src.data.utils;
using static envmanager.src.data.service.dtos.AuthDtos;
using MongoDB.Driver;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.data.infra.db;

namespace envmanager.src.data.service.repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ITokenFactory _jwtService;

        public AuthRepository(AppDbContext db, ITokenFactory jwtService)
        {
            _appDbContext = db;
            _jwtService = jwtService;
        }

        public async Task<string> Login(LoginRequest loginRequest, string refreshToken)
        {
            var user = await _appDbContext.Users
                .Find(u => u.Email == loginRequest.email)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var securityService = new SecurityService();
            bool isPasswordValid = securityService.VerifyPassword(user.Password, loginRequest.password);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var update = Builders<User>.Update
                .Set(u => u.RefreshToken, refreshToken)
                .Set(u => u.RefreshTokenExpiry, DateTime.UtcNow.AddDays(7));

            await _appDbContext.Users.UpdateOneAsync(u => u.Id == user.Id, update);

            return _jwtService.CreateUserToken(user);
        }

        public async Task<string> ValidateRefreshToken(string refreshToken)
        {
            var user = await _appDbContext.Users
                .Find(u => u.RefreshToken == refreshToken)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new UnauthorizedAccessException("Refresh token not found or invalid.");
            }

            if (user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Session has expired. Please login again.");
            }

            return _jwtService.CreateUserToken(user);
        }
    }
}