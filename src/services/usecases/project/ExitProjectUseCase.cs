using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.services.interfaces.project;
using static GlobalExceptionHandler;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace envmanager.src.services.usecases.project
{
    public class ExitProjectUseCase : IExitProjectUseCase
    {
        private readonly IProjectRepository _projectRepository;
        public ExitProjectUseCase(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<bool> Execute(string projectId, string userId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                throw new ArgumentException("User ID and Project ID are required.");

            var project = await _projectRepository.GetProjectById(projectId);
            if (project == null)
                throw new BusinessException("Project not found.");

            var members = project.Members ?? new List<ProjectMember>();
            var memberToRemove = members.FirstOrDefault(m => m.Id == userId);

            if (memberToRemove == null && project.UserId != userId)
                throw new BusinessException("User is not a member of this project.");

            if (project.UserId == userId)
            {
                var successor = members
                    .Where(m => m.Id != userId)
                    .OrderByDescending(m => m.isAdmin) 
                    .FirstOrDefault();

                if (successor != null)
                {
                    await _projectRepository.UpdateOwner(projectId, successor.Id);
                    await _projectRepository.SetMemberAdmin(projectId, successor.Id, true);
                }
                else
                {
                    throw new BusinessException("You are the only member. Delete the project instead of leaving.");
                }
            }

            return await _projectRepository.ExitProject(projectId, userId);
        }
    }
}
