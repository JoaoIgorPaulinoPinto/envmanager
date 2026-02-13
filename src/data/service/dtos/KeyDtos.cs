using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.dtos
{
    public class KeyDTos
    {
        public record CreateVariableRequest
        {
            public string? id { get; set; } = string.Empty;
            [Required(ErrorMessage = "The variable name is required.")]
            public string variable { get; set; } = string.Empty;
            [Required(ErrorMessage = "The variable value is required.")]
            public string Value { get; set; } = string.Empty;
        }
        public record GetVariableResponse
        {
            public string id { get; set; } = string.Empty;
            public string variable { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }
    }
}
