using envmanager.src.data.infra.db;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using MongoDB.Driver;

namespace envmanager.src.data.service.repositories
{
    public class InviteRepository : IInviteRepository
    {
        private readonly AppDbContext _context;

        public InviteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Invite> CreateInvite(Invite invite)
        {
            await _context.Invites.InsertOneAsync(invite);
            return invite;
        }

        public async Task<Invite?> GetByProjectAndInvited(string projectId, string invitedUserId)
        {
            return await _context.Invites
                .Find(i => i.ProjectId == projectId && i.InvitedUserId == invitedUserId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteById(string inviteId)
        {
            var result = await _context.Invites.DeleteOneAsync(i => i.Id == inviteId);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteByProjectAndInvited(string projectId, string invitedUserId)
        {
            var result = await _context.Invites.DeleteOneAsync(i => i.ProjectId == projectId && i.InvitedUserId == invitedUserId);
            return result.DeletedCount > 0;
        }
    }
}
