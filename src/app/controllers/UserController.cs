using envmanager.src.services.interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static envmanager.src.infra.dtos.UsersDtos;

namespace envmanager.src.app.controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class UserController : ControllerBase
    {
        private readonly IGetUsersUseCase _getUsersUseCase;
        private readonly ICreateUserUseCase _createUsersUseCase;
        public UserController(IGetUsersUseCase getUsersUseCase) { 
        
            _getUsersUseCase = getUsersUseCase;
        }
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _getUsersUseCase.Execute());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> Get([FromQuery] string id)
        {
            return Ok(await _getUsersUseCase.Execute(id));
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateUsersRequest user)
        {
            return Ok(await _createUsersUseCase.Execute(user));
        }
    }
}
