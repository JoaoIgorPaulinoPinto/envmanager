using envmanager.src.infra.interfaces;
using envmanager.src.services.interfaces.user;
using static envmanager.src.infra.dtos.UsersDtos;

namespace envmanager.src.services.usecases.user
{
    public class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public CreateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> Execute(CreateUserRequest user)
        {
            if (user == null)
                throw new ArgumentException("User data is required.");

            string token = await _userRepository.Create(user);

            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("User was created, but the access token could not be generated.");
            }

            return token;
        }
    }
}