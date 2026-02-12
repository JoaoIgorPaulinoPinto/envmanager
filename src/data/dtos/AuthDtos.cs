using Microsoft.AspNetCore.Identity.Data;

namespace envmanager.src.data.dtos
{
    public sealed class AuthDtos
    {
       public record LoginRequest {
            public string email { get; set; } = "";
            public string password { get; set; } = "";
        } 
    }
}
