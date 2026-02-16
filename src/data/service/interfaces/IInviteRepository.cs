using envmanager.src.data.service.schemes;

namespace envmanager.src.data.service.interfaces
{
    public interface IInviteRepository
    {
        Task<Invite> CreateInvite(Invite invite);
        Task<Invite?> GetByProjectAndInvited(string projectId, string invitedUserId);
        Task<bool> DeleteById(string inviteId);
        Task<bool> DeleteByProjectAndInvited(string projectId, string invitedUserId);
    }
}
