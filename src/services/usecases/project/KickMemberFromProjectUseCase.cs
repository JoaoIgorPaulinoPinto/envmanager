using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.services.interfaces.project;
using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.usecases.project
{
    public class KickMemberFromProjectUseCase : IKickMemberFromProjectUseCase
    {
        private readonly IProjectRepository _projectRepository;

        public KickMemberFromProjectUseCase(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<bool> Execute(KickMemberFromProjectRequest request, string requesterUserId)
        {
            var project = await _projectRepository.GetProjectById(request.project_id)
                          ?? throw new KeyNotFoundException("Project not found.");

            var requester = project.Members.FirstOrDefault(m => m.Id == requesterUserId);
            bool isOwner = project.UserId == requesterUserId;
            bool isAdmin = requester?.isAdmin ?? false;

            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("You do not have permission to kick members.");
            }

            bool targetIsMember = project.Members.Any(m => m.Id == request.member_id);
            if (!targetIsMember)
            {
                throw new ArgumentException("The specified member is not part of the project.");
            }

            return await _projectRepository.RemoveMember(request.project_id, request.member_id);
        }
    }
}
