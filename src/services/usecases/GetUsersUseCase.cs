using envmanager.src.infra.dtos;
using envmanager.src.services.interfaces;

namespace envmanager.src.services.usecases
{
    public class GetUsersUseCase : IGetUsersUseCase
    {
        private readonly IUserRepository _userRepository;
        public class GetUsersUseCase(IUserRepository userRepository)
        {
            _userRepository =userRepository;
        }
        public Task<List<UsersDtos.GetUsersResponse>> Execute()
        {
            throw new NotImplementedException();
        }

        public Task<List<UsersDtos.GetUsersResponse>> Execute(string id)
        {
            throw new NotImplementedException();
        }
    }
}
