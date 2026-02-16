using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.data.utils;
using envmanager.src.services.interfaces.project;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.usecases.project
{
    public class CreateProjectUseCase : ICreateProjectUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly SecurityService _securityService;

        public CreateProjectUseCase(IProjectRepository projectRepository, SecurityService securityService)
        {
            _projectRepository = projectRepository;
            _securityService = securityService;
        }

        public async Task<bool> Execute(CreateProjectRequest createProjectRequest, string userId)
        {
            if (createProjectRequest == null)
                throw new ArgumentException("Project data is required.");

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("Invalid user on the token.");

            if (string.IsNullOrWhiteSpace(createProjectRequest.name) || string.IsNullOrWhiteSpace(createProjectRequest.description))
                throw new ArgumentException("Project name and description are required.");

            var project = new Project
            {
                Description = createProjectRequest.description,
                ProjectName = createProjectRequest.name,
                Password = string.IsNullOrWhiteSpace(createProjectRequest.password)
                    ? string.Empty
                    : _securityService.HashPassword(createProjectRequest.password),
                UserId = userId,
                Members = [new ProjectMember { Id = userId, isAdmin = true }]
            };

            return await _projectRepository.CreateProject(project);
        }
    }
}
