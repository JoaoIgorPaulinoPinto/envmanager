using envmanager.src.data.interfaces;
using envmanager.src.data.utils;
using envmanager.src.infra.db;
using static envmanager.src.data.dtos.AuthDtos;
using MongoDB.Driver;
using envmanager.src.data.schemes;

namespace envmanager.src.data.repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly JWTService _jwtService;

        public AuthRepository(AppDbContext db, JWTService jwtService)
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
            bool isPasswordValid = securityService.VerificarSenha(user.Password, loginRequest.password);

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