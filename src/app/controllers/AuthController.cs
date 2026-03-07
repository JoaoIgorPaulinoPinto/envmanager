using envmanager.src.services.interfaces.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static envmanager.src.data.service.dtos.AuthDtos;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IAuthLoginUseCase _authLoginUseCase;
    private readonly IValidateRefreshToken _validateRefreshToken;
    private readonly IAuthLogoutUseCase _authLogoutUseCase;

    public AuthController(
        IValidateRefreshToken validateRefreshToken,
        IAuthLoginUseCase authLoginUseCase,
        IAuthLogoutUseCase authLogoutUseCase)
    {
        _authLoginUseCase = authLoginUseCase;
        _validateRefreshToken = validateRefreshToken;
        _authLogoutUseCase = authLogoutUseCase;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var session = await _authLoginUseCase.Execute(loginRequest);
        SetRefreshTokenCookie(session.refreshToken, session.refreshTokenExpiresAtUtc);

        return Ok(new
        {
            message = "Login successfully",
            token = session.token,
            refreshToken = session.refreshToken,
            refreshTokenExpiresAtUtc = session.refreshTokenExpiresAtUtc
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshSessionToken([FromBody] object? payload = null)
    {
        var refreshToken = ResolveRefreshToken(payload);
        var session = await _validateRefreshToken.Execute(refreshToken);

        SetRefreshTokenCookie(session.refreshToken, session.refreshTokenExpiresAtUtc);

        return Ok(new
        {
            message = "Refresh successfully",
            token = session.token,
            refreshToken = session.refreshToken,
            refreshTokenExpiresAtUtc = session.refreshTokenExpiresAtUtc
        });
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] object? payload = null)
    {
        var refreshToken = ResolveRefreshToken(payload, required: false);
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _authLogoutUseCase.Execute(refreshToken);
        }

        DeleteRefreshTokenCookie();

        return Ok(new { message = "Logout successfully" });
    }

    private string ResolveRefreshToken(object? payload, bool required = true)
    {
        var cookieToken = Request.Cookies[RefreshTokenCookieName];
        if (!string.IsNullOrWhiteSpace(cookieToken))
            return cookieToken;

        var payloadToken = ExtractRefreshTokenFromPayload(payload);
        if (!string.IsNullOrWhiteSpace(payloadToken))
            return payloadToken;

        if (required)
            throw new UnauthorizedAccessException("Refresh token is missing.");

        return string.Empty;
    }

    private static string? ExtractRefreshTokenFromPayload(object? payload)
    {
        if (payload is not JsonElement json)
            return null;

        if (json.ValueKind == JsonValueKind.String)
            return json.GetString();

        if (json.ValueKind == JsonValueKind.Object &&
            json.TryGetProperty("refreshToken", out var refreshTokenNode) &&
            refreshTokenNode.ValueKind == JsonValueKind.String)
        {
            return refreshTokenNode.GetString();
        }

        return null;
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAtUtc)
    {
        var isHttpsRequest = Request.IsHttps;
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            // Em HTTP (dev/LAN), Secure=true impede envio do cookie.
            // Em HTTPS, usamos None para permitir cenarios cross-origin com credentials.
            Secure = isHttpsRequest,
            SameSite = isHttpsRequest ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = expiresAtUtc,
            IsEssential = true
        };

        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    private void DeleteRefreshTokenCookie()
    {
        var isHttpsRequest = Request.IsHttps;
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttpsRequest,
            SameSite = isHttpsRequest ? SameSiteMode.None : SameSiteMode.Lax,
            IsEssential = true
        };

        Response.Cookies.Delete(RefreshTokenCookieName, cookieOptions);
    }
}
