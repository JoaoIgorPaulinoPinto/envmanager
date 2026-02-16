using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.data.utils;
using envmanager.src.services.interfaces.user;
using static envmanager.src.data.service.dtos.UsersDtos;

namespace envmanager.src.services.usecases.user
{
    public class CreateUserUseCase : ICreateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly SecurityService _securityService;
        private readonly ITokenFactory _tokenFactory;

        public CreateUserUseCase(IUserRepository userRepository, SecurityService securityService, ITokenFactory tokenFactory)
        {
            _userRepository = userRepository;
            _securityService = securityService;
            _tokenFactory = tokenFactory;
        }

        public async Task<string> Execute(CreateUserRequest user)
        {
            if (user == null)
                throw new ArgumentException("User data is required.");

            if (string.IsNullOrWhiteSpace(user.user_name) || string.IsNullOrWhiteSpace(user.email) || string.IsNullOrWhiteSpace(user.password))
                throw new ArgumentException("User name, email and password are required.");

            var exists = await _userRepository.ExistsByEmail(user.email);
            if (exists)
                throw new InvalidOperationException("A user with this email already exists.");

            var newUser = new User
            {
                UserName = user.user_name,
                Email = user.email,
                Password = _securityService.HashPassword(user.password),
                RefreshToken = _tokenFactory.GenerateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
            };

            var created = await _userRepository.Create(newUser);
            var token = _tokenFactory.CreateUserToken(created);

            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("User was created, but the access token could not be generated.");

            return token;
        }
    }
}
