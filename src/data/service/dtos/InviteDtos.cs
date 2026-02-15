using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.dtos
{
    public class InviteDtos
    {
        public record CreateInviteRequest {

            [Required(ErrorMessage ="Invited user is required")]
            public string invited_user_id { get; set; } = "";
            [Required(ErrorMessage = "Inviter user is required")]
            public string inviter_user_id { get; set; } = ""; 
            [Required(ErrorMessage = "Project is required")]
            public string project_id { get; set; } = "";
        }
        public record CreateInviteResponse {
            public string id { get; set; } = "";
            public string invited_user { get; set; } = "";
            public string project { get; set; } = "";
            public string jwt_token { get; set; } = ""; // guarda o tempo até a invalidez e o usuario que foi convidado
            public bool isValid { get; set; } = false;
        }
    }
}
