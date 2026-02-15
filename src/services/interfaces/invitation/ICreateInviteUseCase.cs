using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.services.usecases.invitation
{
    public interface ICreateInviteUseCase
    {
        public Task<CreateInviteResponse> Execute(CreateInviteRequest request);
    }
}
