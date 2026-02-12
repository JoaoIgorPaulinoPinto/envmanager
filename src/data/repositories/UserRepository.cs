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
        public UserRepository(AppDbContext db, JWTService jwtService) {
            _appDbContext = db;
            _jwtService = jwtService;
        }


        public async Task<List<UsersDtos.GetUsersResponse>> GetAll()
        {
            List<User> users = await _appDbContext.Users.Find(u => true).ToListAsync();
            UserMapping mapper = new UserMapping();
            if(users != null)
            {
                List<UsersDtos.GetUsersResponse> dtosUserList = new();
                foreach (var item in users)
                {
                    UsersDtos.GetUsersResponse userDto = mapper.SchemaToDTO(item);
                    dtosUserList.Add(userDto);
                }
                return dtosUserList;
            }
            else
            {
                throw new Exception("No users found");
            }
                
        }

        public async Task<UsersDtos.GetUsersResponse> GetById(string id)
        {
            User user = await _appDbContext.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
            UserMapping mapper = new UserMapping();
            if (user != null)
            {
               return mapper.SchemaToDTO(user);
            }
            else
            {
                throw new Exception("No users found");
            }
        }
        public async Task<string> Create(UsersDtos.CreateUserRequest user)
        {
           string pwd = new SecurityService().GerarHash(user.password);
             User u = new User();
            u.Password = pwd;
            u.UserName = user.user_name;
            u.Email = user.email;

            u.RefreshToken = _jwtService.GenerateRefreshToken();
            Console.WriteLine(u.RefreshToken);
            await _appDbContext.Users.InsertOneAsync(u);

            string token = _jwtService.CreateUserToken(u);
            if (token == null) throw new Exception("Error on creating the session token");
            return token;
        }
    }
}
