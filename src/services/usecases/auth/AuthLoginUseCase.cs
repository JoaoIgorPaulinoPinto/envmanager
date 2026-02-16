using envmanager.src.data.service.interfaces;
using envmanager.src.data.utils;
using envmanager.src.services.interfaces.auth;
using static envmanager.src.data.service.dtos.AuthDtos;

public class AuthLoginUseCase : IAuthLoginUseCase
{
    private readonly IAuthRepository _authRepo;
    private readonly SecurityService _securityService;
    private readonly ITokenFactory _tokenFactory;

    public AuthLoginUseCase(IAuthRepository authRepo, SecurityService securityService, ITokenFactory tokenFactory)
    {
        _authRepo = authRepo;
        _securityService = securityService;
        _tokenFactory = tokenFactory;
    }

    public async Task<string> Execute(LoginRequest loginRequest, string refreshToken)
    {
        if (loginRequest == null)
            throw new ArgumentException("Login details not provided.");

        if (string.IsNullOrWhiteSpace(loginRequest.email) || string.IsNullOrWhiteSpace(loginRequest.password))
            throw new ArgumentException("Email and password are required.");

        var user = await _authRepo.GetUserByEmail(loginRequest.email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var isPasswordValid = _securityService.VerifyPassword(user.Password, loginRequest.password);
        if (!isPasswordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        await _authRepo.UpdateRefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        return _tokenFactory.CreateUserToken(user);
    }
}
