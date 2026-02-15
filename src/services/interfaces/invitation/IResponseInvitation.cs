using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.services.usecases.invitation
{
    public interface IResponseInvitation
    {
        public Task<ResponseInviteResponse> Execute(ResponseInviteRequest response);
    }
}
