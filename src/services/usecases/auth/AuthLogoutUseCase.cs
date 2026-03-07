using envmanager.src.data.service.interfaces;
using envmanager.src.data.utils;
using envmanager.src.services.interfaces.auth;

namespace envmanager.src.services.usecases.auth
{
    public class AuthLogoutUseCase : IAuthLogoutUseCase
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenFactory _tokenFactory;

        public AuthLogoutUseCase(IAuthRepository authRepository, ITokenFactory tokenFactory)
        {
            _authRepository = authRepository;
            _tokenFactory = tokenFactory;
        }

        public async Task Execute(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var refreshTokenHash = _tokenFactory.HashRefreshToken(refreshToken);
            var user = await _authRepository.GetUserByRefreshToken(refreshTokenHash);
            if (user == null)
                return;

            await _authRepository.RevokeRefreshToken(user.Id);
        }
    }
}
