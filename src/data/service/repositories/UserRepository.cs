using envmanager.src.data.infra.db;
using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.mappers;
using envmanager.src.data.service.schemes;
using MongoDB.Driver;

namespace envmanager.src.data.service.repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserMapping _mapper;

        public UserRepository(AppDbContext db)
        {
            _appDbContext = db;
            _mapper = new UserMapping();
        }

        public async Task<List<UsersDtos.GetUsersResponse>> GetAll()
        {
            var users = await _appDbContext.Users.Find(u => true).ToListAsync();
            return users.Select(user => _mapper.SchemaToDTO(user)).ToList();
        }

        public async Task<UsersDtos.GetUsersResponse?> GetById(string id)
        {
            var user = await _appDbContext.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
            return user == null ? null : _mapper.SchemaToDTO(user);
        }

        public async Task<User?> GetSchemaById(string id)
        {
            return await _appDbContext.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetSchemaByEmail(string email)
        {
            return await _appDbContext.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            return await _appDbContext.Users.Find(u => u.Email == email).AnyAsync();
        }

        public async Task<User> Create(User user)
        {
            await _appDbContext.Users.InsertOneAsync(user);
            return user;
        }
    }
}
