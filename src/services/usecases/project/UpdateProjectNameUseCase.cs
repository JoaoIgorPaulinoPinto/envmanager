using envmanager.src.data.service.interfaces;
using envmanager.src.services.interfaces.project;

namespace envmanager.src.services.usecases.project
{
    public class UpdateProjectNameUseCase : IUpdateProjectNameUseCase
    {
        private readonly IProjectRepository _projectRepository;

        public UpdateProjectNameUseCase(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> Execute(string name, string projectId, string userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name must be provided and cannot be empty.");

            if (string.IsNullOrWhiteSpace(projectId))
                throw new ArgumentException("Project ID must be provided.");

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("Invalid user on the token.");

            var project = await _projectRepository.GetProjectById(projectId);
            if (project == null)
                throw new KeyNotFoundException("Project not found.");

            var member = project.Members.FirstOrDefault(m => m.Id == userId);
            var isOwner = project.UserId == userId;
            var isAdminMember = member?.isAdmin == true;
            if (!isOwner && !isAdminMember)
                throw new UnauthorizedAccessException("Only admin members can change this");

            return await _projectRepository.UpdateName(name, projectId);
        }
    }
}
