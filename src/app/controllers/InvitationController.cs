using envmanager.src.services.interfaces;
using envmanager.src.services.usecases.invitation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static envmanager.src.data.service.dtos.InviteDtos;

namespace envmanager.src.app.controllers
{
    [ApiController]
    [Route("invite")]
    public class InvitationController : ControllerBase
    {
        private readonly ICreateInviteUseCase _createInvitationUseCase;
        private readonly IResponseInvitation _acceptProjectInvite;

        public InvitationController(ICreateInviteUseCase createInvitationUseCase, IResponseInvitation acceptProjectInvite)
        {
            _acceptProjectInvite = acceptProjectInvite;
            _createInvitationUseCase = createInvitationUseCase;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateInvite([FromBody] CreateInviteRequest request)
        {
            var result = await _createInvitationUseCase.Execute(request);

            return Ok(new
            {
                Message = "Invite created successfully",
                Data = result
            });
        }

        [Authorize]
        [HttpPost("answer")] 
        public async Task<IActionResult> AcceptInvite([FromBody] ResponseInviteRequest response)
        {
            var result = await _acceptProjectInvite.Execute(response);
            return Ok(new
            {
                Message = result.message,
                Data = result.accepted
            });
        }
    }
}