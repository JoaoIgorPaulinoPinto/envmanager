using envmanager.src.services.interfaces.user;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.service.dtos.UsersDtos;

[ApiController]
[Route("user")]
public sealed class UserController : ControllerBase
{
    private readonly IGetUsersUseCase _getUsersUseCase;
    private readonly ICreateUserUseCase _createUsersUseCase;

    public UserController(IGetUsersUseCase getUsersUseCase, ICreateUserUseCase createUserUseCase)
    {
        _getUsersUseCase = getUsersUseCase;
        _createUsersUseCase = createUserUseCase;
    }
    private string userId => User.Claims.FirstOrDefault(c => c.Type == "id")?.Value
                           ?? throw new UnauthorizedAccessException("Invalid user on the token.");

    [Authorize]
    [HttpGet("all")]
    public async Task<ActionResult<List<GetUsersResponse>>> GetAll()
    {
        return Ok(await _getUsersUseCase.Execute());
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<GetUsersResponse>> Get()
    {
        var user = await _getUsersUseCase.Execute(userId);
        return Ok(new { message = "User successfully listed", user });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<string>> Create([FromBody] CreateUserRequest user)
    {
        var token = await _createUsersUseCase.Execute(user);
        return CreatedAtAction(nameof(Get), new { message = "User successfully registered.", token });
    }
}