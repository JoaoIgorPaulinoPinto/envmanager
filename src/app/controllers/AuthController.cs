using envmanager.src.data.utils;
using envmanager.src.services.interfaces.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.dtos.AuthDtos;

namespace envmanager.src.app.controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthLoginUseCase _authLoginUseCase;
        private readonly IValidateRefreshToken _validadeRefreshToken;
        private readonly JWTService _JWTService;
        public AuthController (IValidateRefreshToken validadeRefreshToken, IAuthLoginUseCase authLoginUseCase, JWTService JWTService)
        {
            _authLoginUseCase = authLoginUseCase;
            _JWTService = JWTService;
            _validadeRefreshToken = validadeRefreshToken;
        }

        [HttpPost]
        public async Task<ActionResult<dynamic>> Login([FromBody] data.dtos.AuthDtos.LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest?.email) ||
                string.IsNullOrWhiteSpace(loginRequest?.password))
            {
                return BadRequest("Email and password are required..");
            }
            var refreshToken = _JWTService.GenerateRefreshToken();
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(7)
            });
            string token = await _authLoginUseCase.Execute(loginRequest);
            return Ok(new
            {
                message = "Login successfully",
                token = token
            });
        }
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<string>> RefreshSessionToken([FromBody]string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest("Refresh token is required");
            }
            string token = await _validadeRefreshToken.Execute(refreshToken);
            if (string.IsNullOrEmpty(token)) throw new Exception(message: "Refresh token is invalid");
            return Ok(new
            {
                message = "Refresh successfully",
                token = token
            }); ;
        }
    }
}
