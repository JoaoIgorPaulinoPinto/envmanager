using envmanager.src.data.infra.db;
using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.data.utils;
using MongoDB.Driver;
using static envmanager.src.data.service.dtos.InviteDtos;
using static GlobalExceptionHandler;

namespace envmanager.src.data.service.repositories
{
    public class InviteRepository : IInviteRepository
    {
        private readonly AppDbContext _context;
        private readonly ITokenFactory _tokenFactory;

        public InviteRepository (AppDbContext context, ITokenFactory jWTFactory)
        {
            _context = context;
            _tokenFactory = jWTFactory;
        }
        public async Task<CreateInviteResponse> AcceptInvite(string jwtToken)
        {
            var principal = _tokenFactory.GetPrincipalFromToken(jwtToken);
            if (principal == null)
            {
                throw new BusinessException("Convite inválido ou expirado.");
            }

            var invitedId = principal.FindFirst("invitedId")?.Value;
            var projectId = principal.FindFirst("projectId")?.Value;

            var project = await _context.Projects.Find(p => p.Id == projectId).FirstOrDefaultAsync();
            var invitedUser = await _context.Users.Find(u => u.Id == invitedId).FirstOrDefaultAsync();

            if (project == null || invitedUser == null)
            {
                throw new BusinessException("Projeto ou Usuário não encontrado.");
            }

            if (project.Members != null && project.Members.Any(m => m.Id == invitedId))
            {
                throw new BusinessException("Você já é um membro deste projeto.");
            }

            var newMember = new ProjectMember
            {
                Id = invitedUser.Id,
                isAdmin = false
            };

            var update = Builders<Project>.Update.Push(p => p.Members, newMember);
            await _context.Projects.UpdateOneAsync(p => p.Id == projectId, update);
            await _context.Invites.DeleteOneAsync(i => i.ProjectId == projectId && i.InvitedUserId == invitedId);

            return new CreateInviteResponse
            {
                project = project.ProjectName,
                invited_user = invitedUser.UserName,
                isValid = false 
            };
        }

        public async Task<CreateInviteResponse> CreateInvite(CreateInviteRequest request)
        {
         
            var inviter = await _context.Users.Find(u => u.Id == request.inviter_user_id).FirstOrDefaultAsync();
            var project = await _context.Projects.Find(p => p.Id == request.project_id).FirstOrDefaultAsync();
            var invited = await _context.Users.Find(u => u.Id == request.invited_user_id).FirstOrDefaultAsync();

            if (inviter == null || project == null || invited == null)
            {
                throw new BusinessException("Um ou mais envolvidos (convidado, convidador ou projeto) não foram encontrados.");
            }

            var membership = project.Members?.Find(m => m.Id == inviter.Id);

            if (membership == null || !membership.isAdmin)
            {
                throw new BusinessException("Apenas administradores do projeto podem enviar convites.");
            }

            string jwt = _tokenFactory.CreateInviteToken(request.inviter_user_id, request.invited_user_id, request.project_id);

            var invite = new Invite
            {
                InvitedUserId = request.invited_user_id,
                InviterUserId = request.inviter_user_id,
                ProjectId = request.project_id,
                CreatedAt = DateTime.UtcNow 
            };

            await _context.Invites.InsertOneAsync(invite);

            return new CreateInviteResponse
            {
                id = invite.Id!,
                invited_user = invited.UserName,
                project = project.ProjectName,
                jwt_token = jwt,
                isValid = true
            };
        }
        public async Task DeclineInvite(string jwtToken)
        {
            var principal = _tokenFactory.GetPrincipalFromToken(jwtToken);
            if (principal == null) throw new BusinessException("Token inválido.");

            var invitedId = principal.FindFirst("invitedId")?.Value;
            var projectId = principal.FindFirst("projectId")?.Value;

            var result = await _context.Invites.DeleteOneAsync(i =>
                i.ProjectId == projectId && i.InvitedUserId == invitedId);

            if (result.DeletedCount == 0)
            {
                throw new BusinessException("Convite não encontrado ou já processado.");
            }
        }

    }
}
