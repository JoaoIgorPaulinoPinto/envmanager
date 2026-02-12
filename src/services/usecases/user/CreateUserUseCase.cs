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
            try
            {
                string token = await _userRepository.Create(user);
                if (token == null) throw new Exception("The token could not be generated");
                return token;
            }
            catch
            {
                throw new Exception("Error creating user");
            }
        }
    }
}
