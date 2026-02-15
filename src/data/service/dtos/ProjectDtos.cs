using envmanager.src.data.service.schemes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static envmanager.src.data.service.dtos.KeyDTos;
using static envmanager.src.data.service.dtos.MemberDtos;

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
            public bool isOwner { get; set; } = false;
            public string id { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string access_link { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
        }
        public record GetProjectByIdResponse
        {
            [Required]
            public string id { get; set; } = string.Empty;
            [Required]
            public string name { get; set; } = string.Empty;
            [Required]
            public string description { get; set; } = string.Empty;
            [Required]
            public bool isOwner { get; set; } = false;
            [Required]
            public string access_link { get; set; } = string.Empty;
            [Required]
            public List<GetMemberResponse> members { get; set; } = new List<GetMemberResponse>();
            [Required]
            public List<GetVariableResponse> variables { get; set; } = [];
        }

        public record UpdateVariablesRequest
        {

            [Required(ErrorMessage = "Project id is required.")]
            public string project_id { get; set; } = string.Empty;
            [Required(ErrorMessage = "Some variable is required to update project's variables.")]
            public List<CreateVariableRequest> variables { get; set; } = [];
        }
        public record UpdateNameAndDescriptionRequest
        {
            public string project_id { get; set; } = string.Empty;
            public string project_name { get; set; } = string.Empty;
            public string project_description { get; set; } = string.Empty;
        }

        /* Criar endpoint que torna um usuario admin (passa o id do usuario que estara na lista de participantes)*/
    }
}
