using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.services.interfaces.project_invite
{
    public interface IResponseInvitationUseCase
    {
        public Task<ResponseInviteResponse> Execute(ResponseInviteRequest response, string clientId);
    }
}
