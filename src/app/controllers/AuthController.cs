using envmanager.src.data.utils;
using envmanager.src.services.interfaces.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.service.dtos.AuthDtos;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthLoginUseCase _authLoginUseCase;
    private readonly IValidateRefreshToken _validateRefreshToken;
    private readonly ITokenFactory _jwtService;

    public AuthController(IValidateRefreshToken validateRefreshToken, IAuthLoginUseCase authLoginUseCase, ITokenFactory jwtService)
    {
        _authLoginUseCase = authLoginUseCase;
        _jwtService = jwtService;
        _validateRefreshToken = validateRefreshToken;
    }

    [HttpPost]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {

        var refreshToken = _jwtService.GenerateRefreshToken();

        SetRefreshTokenCookie(refreshToken);

        string token = await _authLoginUseCase.Execute(loginRequest, refreshToken);

        return Ok(new
        {
            message = "Login successfully",
            token = token
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshSessionToken([FromBody] string refreshToken)
    {
        string token = await _validateRefreshToken.Execute(refreshToken);

        return Ok(new
        {
            message = "Refresh successfully",
            token = token
        });
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}