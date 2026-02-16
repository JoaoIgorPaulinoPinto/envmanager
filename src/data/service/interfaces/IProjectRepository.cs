using envmanager.src.data.service.schemes;

namespace envmanager.src.data.service.interfaces
{
    public interface IProjectRepository
    {
        Task<bool> CreateProject(Project project);
        Task<List<Project>> GetProjectsByUser(string userId);
        Task<Project?> GetProjectById(string projectId);
        Task<bool> UpdateVariables(string projectId, List<Key> variables);
        Task<bool> UpdateName(string name, string projectId);
        Task<bool> UpdateDescription(string description, string projectId);
        Task<bool> SetMemberAdmin(string projectId, string userId, bool isAdmin);
        Task<bool> AddMember(string projectId, ProjectMember member);
    }
}
