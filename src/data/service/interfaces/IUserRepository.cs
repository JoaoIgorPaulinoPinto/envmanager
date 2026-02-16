using envmanager.src.data.service.schemes;
using static envmanager.src.data.service.dtos.UsersDtos;

namespace envmanager.src.data.service.interfaces
{
    public interface IUserRepository
    {
        Task<List<GetUsersResponse>> GetAll();
        Task<GetUsersResponse?> GetById(string id);
        Task<User?> GetSchemaById(string id);
        Task<User?> GetSchemaByEmail(string email);
        Task<bool> ExistsByEmail(string email);
        Task<User> Create(User user);
    }
}
