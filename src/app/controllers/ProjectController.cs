using envmanager.src.services.interfaces;
using envmanager.src.services.interfaces.project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.app.controllers
{
    [ApiController]
    [Route("project")] // Rota explícita em minúsculo é boa prática
    public class ProjectController : ControllerBase
    {
        private readonly ICreateProjectUseCase _createProjectUseCase;
        private readonly IGetProjectsUseCase _getProjectsUseCase;
        private readonly IUpdateProjectVariables _updateProjectVariables;

        public ProjectController(IUpdateProjectVariables updateProjectVariables, IGetProjectsUseCase getProjectsUseCase, ICreateProjectUseCase createProjectUseCase)
        {
            _createProjectUseCase = createProjectUseCase;
            _getProjectsUseCase = getProjectsUseCase;
            _updateProjectVariables = updateProjectVariables;
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateProjectRequest createProjectRequest)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }
            var created = await _createProjectUseCase.Execute(createProjectRequest, userId);

            if (!created)
            {
                throw new Exception();
            }

            return Created("", new { message = "Project successfully created" });
        }
        [Authorize]
        [HttpGet]
        public async Task<List<GetProjectsResponse>> Get()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }
            return await _getProjectsUseCase.Execute(userId);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<GetProjectByIdResponse> Get([FromRoute]string id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }
            return await _getProjectsUseCase.Execute(userId, id);
        }
        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateVariables([FromBody] UpdateVariablesRequest updateVariablesRequest)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value; 
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }
            var updated = await _updateProjectVariables.Execute(updateVariablesRequest, userId);

            if(updated) return Ok(new { message = "Project successfully created" });
            else
            {
                throw new Exception();
            }
             
        }
    }
}