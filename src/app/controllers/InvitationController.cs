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
        private readonly IAcceptProjectInvite _acceptProjectInvite;

        public InvitationController(ICreateInviteUseCase createInvitationUseCase, IAcceptProjectInvite acceptProjectInvite)
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
        [HttpGet("accept/{token}")] // Adicionado o parâmetro de rota
        public async Task<IActionResult> AcceptInvite([FromRoute] string token)
        {
            var result = await _acceptProjectInvite.Execute(token);

            return Ok(new
            {
                Message = "Invite accepted successfully",
                Data = result
            });
        }
    }
}