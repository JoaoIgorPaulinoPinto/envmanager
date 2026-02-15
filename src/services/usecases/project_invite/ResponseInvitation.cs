using envmanager.src.data.service.dtos;
using envmanager.src.data.service.interfaces;
using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.services.usecases.invitation
{
    public class ResponseInvitation : IResponseInvitation
    {
        private readonly IInviteRepository _inviteRepository;

        public ResponseInvitation (IInviteRepository inviteRepository)
        {
            _inviteRepository = inviteRepository;
        }
        public Task<ResponseInviteResponse> Execute(ResponseInviteRequest response)
        {
            return _inviteRepository.AnswerInvite(response);
        }

    }
}
