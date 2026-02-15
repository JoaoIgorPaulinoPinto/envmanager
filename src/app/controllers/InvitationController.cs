using envmanager.src.services.interfaces;
using envmanager.src.services.interfaces.project;
using envmanager.src.services.usecases.invitation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.app.controllers
{
    [ApiController]
    [Route("invite")]
    public class InvitationController
    {
        private readonly ICreateInviteUseCase _createInvitationUseCase;
        private readonly IAcceptProjectInvite _acceptProjectInvite;
        public InvitationController(ICreateInviteUseCase createInvitationUseCase, IAcceptProjectInvite acceptProjectInvite)
        {
            _acceptProjectInvite = acceptProjectInvite;
            _createInvitationUseCase = createInvitationUseCase;
        }


        [Authorize]
        [HttpPost]
        public Task<CreateInviteResponse> CreateInvite([FromBody]CreateInviteRequest request)
        {
            return _createInvitationUseCase.Execute(request);
        }
        [Authorize]
        [HttpGet]
        public Task<CreateInviteResponse> AcceptInvite([FromRoute]string token)
        {
            return _acceptProjectInvite.Execute(token);
        }
    }
}
