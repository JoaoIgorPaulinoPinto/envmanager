using envmanager.src.services.interfaces.user;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.infra.dtos.UsersDtos;

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
        var user = await _getUsersUseCase.Execute(id);
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