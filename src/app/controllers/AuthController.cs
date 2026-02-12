using envmanager.src.services.interfaces.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.dtos.AuthDtos;

namespace envmanager.src.app.controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthLoginUseCase _authLoginUseCase;
        public AuthController (IAuthLoginUseCase authLoginUseCase)
        {
            _authLoginUseCase = authLoginUseCase;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> Login([FromBody]LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest?.email) ||
                string.IsNullOrWhiteSpace(loginRequest?.password))
            {
                return BadRequest("Email and password are required..");
            }
            return await _authLoginUseCase.Execute(loginRequest);
        }
    }
}
