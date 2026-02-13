using envmanager.src.data.service.schemes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static envmanager.src.data.service.dtos.KeyDTos;

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
            [PasswordPropertyText]
            public string? password { get; set; } = string.Empty;
        }
        public record GetProjectsResponse
        {
            public string id { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
        }
        public record GetProjectByIdResponse
        {
            public string id { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
            public List<GetVariableResponse> variables { get; set; } = [];
        }

        public record UpdateVariablesRequest
        {
   
            [Required(ErrorMessage = "Project id is required.")]
            public string project_id { get; set; } = string.Empty;
            [Required(ErrorMessage = "Some variable is required to update project's variables.")]
            public List<CreateVariableRequest> variables { get; set; } = [];
        }
        public record UpdateVariablesResponse
        {
           public string message { get; } = "Updated sucefully!";
        }
    }
}
