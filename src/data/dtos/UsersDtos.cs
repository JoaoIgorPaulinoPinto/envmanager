namespace envmanager.src.infra.dtos
{
    public sealed class UsersDtos
    {
        public record GetUsersResponse
        {
            public string id { get; set; } = "";
            public string user_name { get; set; } = "";
            public string password { get; set; } = "";
            public string email { get; set; } = "";
        }
        public record CreateUsersRequest
        {
            public string user_name { get; set; } = "";
            public string password { get; set; } = "";
            public string email { get; set; } = "";
        }
    }
}
