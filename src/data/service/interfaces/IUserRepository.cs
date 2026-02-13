using static envmanager.src.data.service.dtos.UsersDtos;

namespace envmanager.src.data.service.interfaces
{
    public interface IUserRepository
    {
        public Task<List<GetUsersResponse>> GetAll();
        public Task<GetUsersResponse> GetById(string id);
        public Task<string> Create(CreateUserRequest user);
    }
}
