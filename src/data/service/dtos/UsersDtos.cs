using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.dtos
{
    public sealed class UsersDtos
    {
        public record CreateUserRequest
        {
            [Required(ErrorMessage = "User name is required")]
            public string user_name { get; set; } = "";

            [Required(ErrorMessage = "Password is required")]
            public string password { get; set; } = "";

            [Required(ErrorMessage = "E-mail is required")]
            public string email { get; set; } = "";
        }
        public record GetUsersResponse
        {
            public string id { get; set; } = "";
            public string user_name { get; set; } = "";
            public string email { get; set; } = "";
        }
    }
}
