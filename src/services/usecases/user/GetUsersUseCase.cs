using envmanager.src.infra.dtos;
using envmanager.src.infra.interfaces;
using envmanager.src.services.interfaces.user;

namespace envmanager.src.services.usecases.user
{
    public class GetUsersUseCase : IGetUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UsersDtos.GetUsersResponse>> Execute()
        {
            return await _userRepository.GetAll();
        }

        public async Task<UsersDtos.GetUsersResponse> Execute(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("The provided User ID is invalid.");
            }
            return await _userRepository.GetById(id);
        }
    }
}