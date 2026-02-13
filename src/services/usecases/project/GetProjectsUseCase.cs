using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.services.interfaces.project;

namespace envmanager.src.services.usecases.project
{
    public class GetProjectsUseCase : IGetProjectsUseCase
    {
        private readonly IProjectRepository _projectsRepository;    
        public GetProjectsUseCase(IProjectRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<List<ProjectDtos.GetProjectsResponse>> Execute(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("The provided User ID is invalid.");
            }
            return await _projectsRepository.GetProjects(userId);
        }
    }
}
