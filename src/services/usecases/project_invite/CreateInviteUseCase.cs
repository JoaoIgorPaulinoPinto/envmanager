using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using envmanager.src.data.service.schemes;
using Microsoft.AspNetCore.SignalR;
using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.services.usecases.invitation
{
    public class CreateInviteUseCase : ICreateInviteUseCase
    {
        private readonly IInviteRepository _inviteRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        public CreateInviteUseCase (IInviteRepository inviteRepository, IHubContext<NotificationHub> hubContext)
        {
            _inviteRepository = inviteRepository;
            _hubContext = hubContext;
        }
        public async Task<CreateInviteResponse> Execute(CreateInviteRequest request, string invitatorId)
        {
            CreateInviteResponse res = await _inviteRepository.CreateInvite(request, invitatorId);
            if (res != null)
            {
                // LOG DE DEBUG
                Console.WriteLine($"Tentando notificar o usuário ID: {request.invited_user_id}");

                await _hubContext.Clients.User(request.invited_user_id).SendAsync("ReceiveInvitation", new
                {
                    res.invited_user,
                    res.project,
                    res.inviter_user,
                    message = "Você recebeu um novo convite de projeto!"
                });
            }
            return res!;
        }

    }
}
