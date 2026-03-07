using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.data.utils;
using Microsoft.AspNetCore.SignalR;
using static envmanager.src.data.service.dtos.InviteDtos;
using static GlobalExceptionHandler;

namespace envmanager.src.services.usecases.invitation
{
    public class CreateInviteUseCase : ICreateInviteUseCase
    {
        private readonly IInviteRepository _inviteRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenFactory _tokenFactory;
        private readonly IHubContext<NotificationHub> _hubContext;

        public CreateInviteUseCase(
            IInviteRepository inviteRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            ITokenFactory tokenFactory,
            IHubContext<NotificationHub> hubContext)
        {
            _inviteRepository = inviteRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _tokenFactory = tokenFactory;
            _hubContext = hubContext;
        }

        public async Task<CreateInviteResponse> Execute(CreateInviteRequest request, string invitatorId)
        {
            if (request == null)
                throw new ArgumentException("Invalid request.");

            if (string.IsNullOrWhiteSpace(request.invited_user_email) || string.IsNullOrWhiteSpace(request.project_id))
                throw new ArgumentException("Invited user and project are required.");

            var inviter = await _userRepository.GetSchemaById(invitatorId);
            var invited = await _userRepository.GetSchemaByEmail(request.invited_user_email);
            var project = await _projectRepository.GetProjectById(request.project_id);

            if (inviter == null || invited == null || project == null)
                throw new BusinessException("One or more parties (guest, inviter, or project) could not be located.");

            if (project.Members.Any(m => m.Id == invited.Id))
                throw new BusinessException("The invited user is already a member of this project.");

            var membership = project.Members.FirstOrDefault(m => m.Id == inviter.Id);
            var isOwner = inviter.Id == project.UserId;
            var isAdminMember = membership?.isAdmin == true;
            if (!isOwner && !isAdminMember)
                throw new BusinessException("Only project administrators can send invitations.");

            var invitedId = invited.Id ?? throw new BusinessException("Invited user id is invalid.");
            if (invitedId == invitatorId)
                throw new BusinessException("You cannot send an invitation to yourself.");

            var existingInvite = await _inviteRepository.GetByProjectAndInvited(request.project_id, invitedId);
            if (existingInvite != null)
                throw new BusinessException("There is already a pending invitation for this user in this project.");

            var jwt = _tokenFactory.CreateInviteToken(invitatorId, invitedId, request.project_id);
            var invite = await _inviteRepository.CreateInvite(new Invite
            {
                InvitedUserId = invitedId,
                InviterUserId = invitatorId,
                ProjectId = request.project_id,
                CreatedAt = DateTime.UtcNow
            });

            var response = new CreateInviteResponse
            {
                id = invite.Id ?? string.Empty,
                invited_user = invited.UserName,
                inviter_user = inviter.UserName,
                project = project.ProjectName,
                jwt_token = jwt,
                isValid = true
            };

            var inviteCreatedPayload = new
            {
                type = "invitation.created",
                invite_id = response.id,
                invited_user_id = invitedId,
                inviter_user_id = invitatorId,
                project_id = request.project_id,
                response.invited_user,
                response.project,
                response.inviter_user,
                token = response.jwt_token,
                created_at_utc = invite.CreatedAt,
                message = "Voce recebeu um novo convite de projeto!"
            };

            await SendNotificationSafely(invitedId, "ReceiveInvitation", inviteCreatedPayload);
            await SendNotificationSafely(invitatorId, "InvitationCreated", inviteCreatedPayload);

            return response;
        }

        private async Task SendNotificationSafely(string userId, string eventName, object payload)
        {
            try
            {
                await _hubContext.Clients.User(userId).SendAsync(eventName, payload);
            }
            catch
            {
                // A notificacao em tempo real nao deve impedir a operacao principal.
            }
        }
    }
}
