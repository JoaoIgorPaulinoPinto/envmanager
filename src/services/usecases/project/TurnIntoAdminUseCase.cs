using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.services.interfaces.project;
using static GlobalExceptionHandler;

namespace envmanager.src.services.usecases.project
{
    public class TurnIntoAdminUseCase : ITurnIntoAdminUseCase
    {
        private readonly IProjectRepository _projectRepository;

        public TurnIntoAdminUseCase(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<bool> Execute(ProjectDtos.ToggleMemberAdminRequest request, string adminId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.project_id) || string.IsNullOrWhiteSpace(request.user_id))
                throw new ArgumentException("Project ID and user ID are required.");

            var project = await _projectRepository.GetProjectById(request.project_id);
            if (project == null)
                throw new BusinessException("Project not found.");

            
            var members = project.Members ?? new List<ProjectMember>(); 
            var caller = members.FirstOrDefault(m => m.Id == adminId);
            var target = members.FirstOrDefault(m => m.Id == request.user_id);

            var callerIsOwner = project.UserId == adminId;
            var callerIsAdmin = caller?.isAdmin == true;

            if (!callerIsOwner && !callerIsAdmin)
                throw new BusinessException("Only project administrators can promote other members.");

            if (target == null)
                throw new BusinessException("The user to be promoted is not a member of this project.");

            if (target.isAdmin == request.to_admin)
                throw new BusinessException($"The user already has the requested status (Admin: {target.isAdmin}).");

            if (target.Id == project.UserId && !request.to_admin)
                throw new BusinessException("It is not possible to remove admin rights from the project owner.");

            return await _projectRepository.SetMemberAdmin(request.project_id, request.user_id, request.to_admin);
        }
    }
}
