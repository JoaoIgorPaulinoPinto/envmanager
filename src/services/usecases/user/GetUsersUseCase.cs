using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
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
            var users = await _userRepository.GetAll();
            if (users.Count == 0)
                throw new KeyNotFoundException("No users found in the database.");

            return users;
        }

        public async Task<UsersDtos.GetUsersResponse> Execute(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("The provided User ID is invalid.");
            }

            var user = await _userRepository.GetById(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} was not found.");

            return user;
        }
    }
}
