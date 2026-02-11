using envmanager.src.infra.dtos;
using envmanager.src.infra.interfaces;
using envmanager.src.services.interfaces;

namespace envmanager.src.services.usecases
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UsersDtos.GetUsersResponse> Execute(string id)
        {
            try
            {
                return await _userRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
