using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.services.interfaces.project;
using MongoDB.Bson.Serialization.Serializers;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.usecases.project
{
    public class DeleteProjectUseCase : IDeleteProjectUseCase
    {
        private readonly IProjectRepository _projectRepository;

        public DeleteProjectUseCase(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public Task<bool> Execute(string projectId)
        {
            bool projectsExist = _projectRepository.GetProjectById(projectId).Result != null;
            bool isOwner = _projectRepository.GetProjectById(projectId).Result?.UserId == projectId;
            bool isAdminMember = _projectRepository.GetProjectById(projectId).Result?.Members.Any(m => m.Id == projectId && m.isAdmin) == true;
            if(projectsExist && (isOwner || isAdminMember))
            {
                return _projectRepository.DeleteProject(projectId);
            }
            else
            {
                if(isOwner || isAdminMember)
                    throw new KeyNotFoundException("Project not found.");
                else
                    throw new UnauthorizedAccessException("You do not have permission to delete this project.");

            }
        }
    }
}
