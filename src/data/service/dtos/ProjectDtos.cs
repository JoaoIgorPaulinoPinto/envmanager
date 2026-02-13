using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.dtos
{
    public class ProjectDtos
    {
        public record CreateProjectRequest
        {
            [Required(ErrorMessage = "Name is required")]
            public string name { get; set; } = string.Empty;
            [Required(ErrorMessage = "Description is required")]
            public string description { get; set; } = string.Empty;
            public string? password { get; set; } = string.Empty;
        }
        public record GetProjectsResponse
        {
            public string id { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
        }
    }
}
