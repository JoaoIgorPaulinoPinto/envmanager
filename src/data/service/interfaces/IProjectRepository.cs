using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.data.service.interfaces
{
    public interface IProjectRepository
    {
        public Task<bool> CreateProject(CreateProjectRequest createProjectRequest, string userId);
        public Task<List<GetProjectsResponse>> GetProjects(string userId);
        public Task<GetProjectByIdResponse> GetProjectById(string userId, string projId);
        public Task<GetProjectByIdResponse> GetProjectById(string userId, string projId, string password);
        public Task<bool> UpdateVariables(UpdateVariablesRequest updateVariablesRequest, string projId);
        public Task<bool> UpdateName(string name, string projId);
        public Task<bool> UpdateDescription(string description, string projId);
        public Task<bool> TurnIntoAdmin(TurnIntoAdminRequest request, string adminId);

    }
}
