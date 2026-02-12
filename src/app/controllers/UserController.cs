using envmanager.src.services.interfaces.user;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.infra.dtos.UsersDtos;

namespace envmanager.src.app.controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class UserController : ControllerBase
    {
        private readonly IGetUsersUseCase _getUsersUseCase;
        private readonly ICreateUserUseCase _createUsersUseCase;

        public UserController(IGetUsersUseCase getUsersUseCase, ICreateUserUseCase createUserUseCase)
        {
            _getUsersUseCase = getUsersUseCase;
            _createUsersUseCase = createUserUseCase;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<GetUsersResponse>>> Get()
        {
            return Ok(await _getUsersUseCase.Execute());
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUsersResponse>> Get([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("User ID is required.");

            return Ok(await _getUsersUseCase.Execute(id));
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody] CreateUserRequest user)
        {
            if (user == null)
                return BadRequest("User data not provided.");
            if (string.IsNullOrWhiteSpace(user.email) || string.IsNullOrWhiteSpace(user.password) || string.IsNullOrWhiteSpace(user.user_name))
            {
                return BadRequest("Email, user name and password are required.");
            }
            return Ok(await _createUsersUseCase.Execute(user));
        }
    }
}