using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.services.usecases.invitation
{
    public class CreateInviteUseCase : ICreateInviteUseCase
    {
        private readonly IInviteRepository _inviteRepository;

        public CreateInviteUseCase (IInviteRepository inviteRepository)
        {
            _inviteRepository = inviteRepository;
        }
        public Task<CreateInviteResponse> Execute(CreateInviteRequest request)
        {
            return _inviteRepository.CreateInvite(request);
        }

    }
}
