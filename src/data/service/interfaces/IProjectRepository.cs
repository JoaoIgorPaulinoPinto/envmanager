using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.data.service.interfaces
{
    public interface IProjectRepository
    {
        public Task<bool> CreateProject(CreateProjectRequest createProjectRequest, string userId);
        public Task<List<GetProjectsResponse>> GetProjects(string userId);
    }
}
