using envmanager.src.infra.dtos;
using envmanager.src.infra.interfaces;
using envmanager.src.services.interfaces.user;

namespace envmanager.src.services.usecases.user
{
    public class GetUsersUseCase : IGetUsersUseCase
    {
        private readonly IUserRepository _userRepository;
        public GetUsersUseCase (IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<UsersDtos.GetUsersResponse>> Execute()
        {
            try
            {
                return await _userRepository.GetAll();
            }
            catch
            {
                throw new Exception("Error on get all users");
            }
        }

        public async Task<UsersDtos.GetUsersResponse> Execute(string id)
        {
            try
            {
                return await _userRepository.GetById(id);
            }
            catch 
            {
                throw new Exception("Error on get user with id: " + id);
            }
        }
    }
}
