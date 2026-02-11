using static envmanager.src.infra.dtos.UsersDtos;

namespace envmanager.src.infra.interfaces
{
    public interface IUserRepository
    {
        public Task<List<GetUsersResponse>> GetAll();
        public Task<GetUsersResponse> GetById(string id);
    }
}
