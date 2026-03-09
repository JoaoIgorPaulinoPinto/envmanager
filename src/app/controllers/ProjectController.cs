using envmanager.src.services.interfaces.project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.app.controllers
{
    [ApiController]
    [Route("project")]
    public class ProjectController : ControllerBase
    {
        private readonly ICreateProjectUseCase _createProjectUseCase;
        private readonly IGetProjectsUseCase _getProjectsUseCase;
        private readonly IDeleteProjectUseCase _deleteProjectUseCase;
        private readonly IUpdateProjectVariablesUseCase _updateProjectVariablesUseCase;
        private readonly IUpdateProjectNameUseCase _updateProjectNameUseCase;
        private readonly IUpdateProjectDescriptionUseCase _updateProjectDescriptionUseCase;
        private readonly ITurnIntoAdminUseCase _turnIntoAdminUseCase;
        private readonly IKickMemberFromProjectUseCase _kickMemberFromProjectUseCase;

        public ProjectController(
            ITurnIntoAdminUseCase turnIntoAdminUseCase,
            IUpdateProjectVariablesUseCase updateProjectVariablesUseCase,
            IDeleteProjectUseCase _deleteProjectUseCase,
            IGetProjectsUseCase getProjectsUseCase,
            ICreateProjectUseCase createProjectUseCase,
            IUpdateProjectNameUseCase updateProjectNameUseCase,
            IDeleteProjectUseCase deleteProjectUseCase,
            IUpdateProjectDescriptionUseCase updateProjectDescriptionUseCase,
            IKickMemberFromProjectUseCase kickMemberFromProjectUseCase
            )
        {
            _turnIntoAdminUseCase = turnIntoAdminUseCase;
            _updateProjectVariablesUseCase = updateProjectVariablesUseCase;
            _updateProjectNameUseCase = updateProjectNameUseCase;
            _createProjectUseCase = createProjectUseCase;
            _getProjectsUseCase = getProjectsUseCase;
            _updateProjectDescriptionUseCase = updateProjectDescriptionUseCase;
            _kickMemberFromProjectUseCase = kickMemberFromProjectUseCase;
        }
            
        private string userId => User.Claims.FirstOrDefault(c => c.Type == "id")?.Value
                           ?? throw new UnauthorizedAccessException("Invalid user on the token.");

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateProjectRequest createProjectRequest)
        {
            await _createProjectUseCase.Execute(createProjectRequest, userId);
            return Created("", new { message = "Project successfully created" });
        }

        [Authorize]
        [HttpGet]
        public async Task<List<GetProjectsResponse>> Get()
        {
            return await _getProjectsUseCase.Execute(userId);
        }

        [Authorize]
        [HttpPost("{id}/details")]
        public async Task<GetProjectByIdResponse> Get([FromRoute] string id, [FromBody] GetProjectDetailsRequest? req)
        {
            if (req == null || string.IsNullOrEmpty(req.password))
            {
                return await _getProjectsUseCase.Execute(userId, id);
            }

            return await _getProjectsUseCase.Execute(userId, id, req.password);
        }

        [Authorize]
        [HttpPut("variables")]
        public async Task<ActionResult> UpdateVariables([FromBody] UpdateVariablesRequest updateVariablesRequest)
        {
            await _updateProjectVariablesUseCase.Execute(updateVariablesRequest, userId);
            return Ok(new { message = "Variables successfully updated" });
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult> UpdateProjectNameAndDescription([FromBody] UpdateNameAndDescriptionRequest updateVariablesRequest)
        {
            bool name = await _updateProjectNameUseCase.Execute(updateVariablesRequest.project_name, updateVariablesRequest.project_id, userId);
            bool desc = await _updateProjectDescriptionUseCase.Execute(updateVariablesRequest.project_description, updateVariablesRequest.project_id, userId);

            if (name || desc)
            {
                return Ok(new { message = "Project successfully updated" });
            }

            return Ok(new { message = "Nothing updated" });
        }

        [Authorize]
        [HttpPut("admin")]
        public async Task<ActionResult> TurnMemberToAdmin([FromBody] TurnIntoAdminRequest request)
        {
            bool result = await _turnIntoAdminUseCase.Execute(request, userId);
            if (result)
            {
                return Ok(new { message = "Member promoted to administrator successfully" });
            }

            return Ok(new { message = "Nothing updated" });
        }
        [Authorize]
        [HttpPut("kick")]
        public async Task<ActionResult> KickMemberFromProject([FromBody] KickMemberFromProjectRequest request)
        {
            bool result = await _kickMemberFromProjectUseCase.Execute(request, userId);
            if (result)
            {
                return Ok(new { message = "Member removed from the project successfully" });
            }

            return Ok(new { message = "Member not removed from the project" });
        }
        [Authorize]
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteProject([FromBody] string projectId)
        {
            bool result = await _deleteProjectUseCase.Execute(projectId);
            if (result)
            {
                return Ok(new { message = "Project deleted successfully" });
            }

            return Ok(new { message = "Project not deleted" });
        }
    }
}
