using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.dtos
{
    public class InviteDtos
    {
        public record CreateInviteRequest
        {

            [Required(ErrorMessage = "Invited user is required")]
            public string invited_user_id { get; set; } = "";
            [Required(ErrorMessage = "Project is required")]
            public string project_id { get; set; } = "";
        }
        public record CreateInviteResponse
        {
            public string id { get; set; } = "";
            public string invited_user { get; set; } = "";
            public string inviter_user { get; set; } = "";
            public string project { get; set; } = "";
            public string jwt_token { get; set; } = ""; 
            public bool isValid { get; set; } = false;
        }
        public record ResponseInviteRequest
        {
            public string token { get; set; } = "";
            public bool accepted { get; set; } = false;
        }
        public record ResponseInviteResponse
        {
            public string message { get; set; } = "";
            public bool accepted { get; set; }
        }
    }
}
