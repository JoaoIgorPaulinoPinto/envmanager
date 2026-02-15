
using envmanager.src.data.service.interfaces;
using envmanager.src.services.interfaces.project;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.usecases.project
{
    public class UpdateProjectVariablesUseCase : IUpdateProjectVariablesUseCase
    {
        private readonly IProjectRepository _projectRepository;
        public UpdateProjectVariablesUseCase(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<bool> Execute(UpdateVariablesRequest updateVariablesRequest, string userId)
        {
            return await _projectRepository.UpdateVariables(updateVariablesRequest, userId);
        }
    }
}       
