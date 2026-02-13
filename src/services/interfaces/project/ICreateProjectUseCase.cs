using envmanager.src.data.service.schemes;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.interfaces.project
{
    public interface ICreateProjectUseCase
    {
        public Task<bool> Execute(CreateProjectRequest createProjectRequest, string userId);
    }
}
