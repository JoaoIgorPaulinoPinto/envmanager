    using envmanager.src.data.service.schemes;
    using envmanager.src.services.interfaces.project;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Primitives;
    using static envmanager.src.data.service.dtos.ProjectDtos;

    namespace envmanager.src.app.controllers
    {
    [ApiController]
    [Route("project")] // Rota explícita em minúsculo é boa prática
    public class ProjectController : ControllerBase
    {
        private readonly ICreateProjectUseCase _createProjectUseCase;
        private readonly IGetProjectsUseCase _getProjectsUseCase;

        public ProjectController(IGetProjectsUseCase getProjectsUseCase, ICreateProjectUseCase createProjectUseCase)
        {
            _getProjectsUseCase = getProjectsUseCase;
            _createProjectUseCase = createProjectUseCase;
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateProjectRequest createProjectRequest)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("The provided User ID is invalid."); 
            var created = await _createProjectUseCase.Execute(createProjectRequest, userId);

            if (!created)
            {
                return BadRequest(new { message = "Could not create project. Verify provided data." });
            }

            return Created("", new { message = "Project successfully created" });
        }
        [Authorize]
        [HttpGet]
        public async Task<List<GetProjectsResponse>> Get()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if(string.IsNullOrEmpty(userId)) throw new ArgumentException("The provided User ID is invalid.");
            return await _getProjectsUseCase.Execute(userId);
        }
    }
    }