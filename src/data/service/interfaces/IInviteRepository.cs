using static envmanager.src.data.service.dtos.InviteDtos;
namespace envmanager.src.data.service.interfaces
{
    public interface IInviteRepository
    {
        public Task<CreateInviteResponse> CreateInvite(CreateInviteRequest request);
        public Task<CreateInviteResponse> AcceptInvite(string request);
        public Task DeclineInvite(string request);
    }
}
