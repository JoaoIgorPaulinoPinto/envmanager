
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.services.interfaces.project;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.usecases.project
{
    public class CreateProjectUseCase : ICreateProjectUseCase
    {
        private readonly IProjectRepository _projectRepository;
        public CreateProjectUseCase(IProjectRepository projectRepository) 
        {
            _projectRepository = projectRepository;
        }
        public async Task<bool> Execute(CreateProjectRequest createProjectRequest, string userId)
        {
            bool success = await _projectRepository.CreateProject(createProjectRequest, userId);
                return success;
        }
    }
}
