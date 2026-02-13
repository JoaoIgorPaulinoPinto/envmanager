using DnsClient;
using envmanager.src.data.service.interfaces;
using envmanager.src.services.interfaces.auth;
using static envmanager.src.data.service.dtos.AuthDtos;

public class AuthLoginUseCase : IAuthLoginUseCase
{
    private readonly IAuthRepository _authRepo;
    public AuthLoginUseCase(IAuthRepository authRepo) { _authRepo = authRepo; }
    public async Task<string> Execute(LoginRequest loginRequest, string refreshToken)
    {
        if (loginRequest == null)
            throw new ArgumentException("Login details not provided.");

        var token = await _authRepo.Login(loginRequest, refreshToken);

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return token;
    }
}