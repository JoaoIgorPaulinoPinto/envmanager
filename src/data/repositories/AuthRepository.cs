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
        public async Task<string> Login(LoginRequest loginRequest)
        {
            var user = await _appDbContext.Users
                .Find(u => u.Email == loginRequest.email)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("E-mail ou senha inválidos.");
            }
            var securityService = new SecurityService();
            bool isPasswordValid = securityService.VerificarSenha(user.Password, loginRequest.password);

            if (!isPasswordValid)
            {
                throw new Exception("E-mail ou senha inválidos.");
            }

            return _jwtService.CreateUserToken(user);
        }
    }
}