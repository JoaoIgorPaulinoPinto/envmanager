using static envmanager.src.data.service.dtos.ProjectDtos;

namespace envmanager.src.services.interfaces.project
{
    public interface IKickMemberFromProjectUseCase
    {
        public Task<bool> Execute(KickMemberFromProjectRequest request, string requesterUserId);
    }
}
