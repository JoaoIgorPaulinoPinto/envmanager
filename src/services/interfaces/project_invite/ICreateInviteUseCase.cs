using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.services.interfaces.project_invite
{
    public interface ICreateInviteUseCase
    {
        public Task<CreateInviteResponse> Execute(CreateInviteRequest request, string invitatorId);
    }
}
