using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using envmanager.src.data.utils;
using Microsoft.AspNetCore.SignalR;
using static envmanager.src.data.service.dtos.InviteDtos;
using static GlobalExceptionHandler;

namespace envmanager.src.services.usecases.invitation
{
    public class ResponseInvitationUseCase : IResponseInvitationUseCase
    {
        private readonly IInviteRepository _inviteRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenFactory _tokenFactory;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ResponseInvitationUseCase(
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

        public async Task<ResponseInviteResponse> Execute(ResponseInviteRequest response, string clientId)
        {
            if (response == null)
                throw new ArgumentException("Invalid request.");

            if (string.IsNullOrWhiteSpace(response.token))
                throw new ArgumentException("Token cannot be null or empty.");

            var principal = _tokenFactory.GetPrincipalFromToken(response.token);
            if (principal == null)
                throw new BusinessException("Invalid or expired invitation token.");

            var invitedId = principal.FindFirst("invitedId")?.Value;
            var inviterId = principal.FindFirst("inviterId")?.Value;
            var projectId = principal.FindFirst("projectId")?.Value;

            if (string.IsNullOrWhiteSpace(invitedId) || string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(inviterId))
                throw new BusinessException("Invalid invitation token payload.");

            if (clientId != invitedId)
                throw new BusinessException("This invitation can only be handled by the intended guest.");

            var inviteRecord = await _inviteRepository.GetByProjectAndInvited(projectId, invitedId);
            if (inviteRecord == null)
                throw new BusinessException("Invitation not found or already processed.");

            if (!response.accepted)
            {
                var deleted = await _inviteRepository.DeleteById(inviteRecord.Id!);
                if (!deleted)
                    throw new BusinessException("Invitation not found or already processed.");

                await NotifyInvitationAnswered(inviterId, invitedId, projectId, false);

                return new ResponseInviteResponse
                {
                    accepted = false,
                    message = "Invitation declined successfully."
                };
            }

            var project = await _projectRepository.GetProjectById(projectId);
            var invitedUser = await _userRepository.GetSchemaById(invitedId);
            if (project == null || invitedUser == null)
                throw new BusinessException("Project or User not found.");

            if (project.Members.Any(m => m.Id == invitedId))
            {
                await _inviteRepository.DeleteById(inviteRecord.Id!);
                throw new BusinessException("You are already a member of this project.");
            }

            await _projectRepository.AddMember(projectId, new ProjectMember
            {
                Id = invitedId,
                isAdmin = false
            });

            await _inviteRepository.DeleteById(inviteRecord.Id!);
            await NotifyInvitationAnswered(inviterId, invitedId, projectId, true);

            return new ResponseInviteResponse
            {
                accepted = true,
                message = "Invitation accepted successfully."
            };
        }

        private async Task NotifyInvitationAnswered(string inviterId, string invitedId, string projectId, bool accepted)
        {
            var project = await _projectRepository.GetProjectById(projectId);
            var invitedUser = await _userRepository.GetSchemaById(invitedId);

            var payload = new
            {
                type = "invitation.answered",
                inviter_user_id = inviterId,
                invited_user_id = invitedId,
                project_id = projectId,
                project_name = project?.ProjectName ?? string.Empty,
                invited_user = invitedUser?.UserName ?? string.Empty,
                accepted,
                answered_at_utc = DateTime.UtcNow,
                message = accepted ? "Invitation accepted successfully." : "Invitation declined successfully."
            };

            await SendNotificationSafely(inviterId, "InvitationAnswered", payload);
            await SendNotificationSafely(invitedId, "InvitationAnswered", payload);
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
