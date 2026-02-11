using envmanager.src.infra.db;
using envmanager.src.infra.dtos;
using envmanager.src.infra.interfaces;
using MongoDB.Driver;

namespace envmanager.src.infra.repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        public UserRepository(AppDbContext db) {
            _appDbContext = db;
        } 

        public async Task<List<UsersDtos.GetUsersResponse>> GetAll()
        {
            var users = _appDbContext.Users.Find(u => true).ToListAsync();
            if(users != null)
                 //return users;
                 //mapear 
        }

        public Task<List<UsersDtos.GetUsersResponse>> GetById()
        {
            throw new NotImplementedException();
        }
    }
}
