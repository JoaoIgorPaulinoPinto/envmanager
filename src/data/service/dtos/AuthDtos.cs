using Microsoft.AspNetCore.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.dtos
{
    public sealed class AuthDtos
    {
       public record LoginRequest {
            [Required(ErrorMessage = "E-mail is required")]
            public string email { get; set; } = "";
            [Required(ErrorMessage = "Password is required")]
            public string password { get; set; } = "";
        } 
    }
}
