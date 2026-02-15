using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.services.usecases.invitation
{
    public interface IAcceptProjectInvite
    {
        public Task<CreateInviteResponse> Execute(string request);
    }
}
