using envmanager.src.data.schemes;
using envmanager.src.data.utils;
using envmanager.src.infra.db;
using envmanager.src.infra.dtos;
using envmanager.src.infra.interfaces;
using envmanager.src.infra.mappers;
using MongoDB.Driver;

namespace envmanager.src.infra.repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly JWTService _jwtService;
        private readonly UserMapping _mapper; 

        public UserRepository(AppDbContext db, JWTService jwtService)
        {
            _appDbContext = db;
            _jwtService = jwtService;
            _mapper = new UserMapping();
        }

        public async Task<List<UsersDtos.GetUsersResponse>> GetAll()
        {
            var users = await _appDbContext.Users.Find(u => true).ToListAsync();

            if (users == null || !users.Any())
            {
                throw new KeyNotFoundException("No users found in the database.");
            }

            return users.Select(user => _mapper.SchemaToDTO(user)).ToList();
        }

        public async Task<UsersDtos.GetUsersResponse> GetById(string id)
        {
            var user = await _appDbContext.Users.Find(u => u.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} was not found.");
            }

            return _mapper.SchemaToDTO(user);
        }

        public async Task<string> Create(UsersDtos.CreateUserRequest userRequest)
        {
            var existingUser = await _appDbContext.Users.Find(u => u.Email == userRequest.email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            string hashedPassword = new SecurityService().GerarHash(userRequest.password);

            User newUser = new User
            {
                Password = hashedPassword,
                UserName = userRequest.user_name,
                Email = userRequest.email,
                RefreshToken = _jwtService.GenerateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7) 
            };

            await _appDbContext.Users.InsertOneAsync(newUser);

            string token = _jwtService.CreateUserToken(newUser);

            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Error generating the session token.");
            }

            return token;
        }
    }
}