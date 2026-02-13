using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.dtos
{
    public class MemberDtos
    {
        public record GetMemberResponse()
        {
            [Required(ErrorMessage ="Member name is required.")]
            public string Name { get; set; } = "";
            [Required(ErrorMessage = "Invalid state to member authorizations.")]
            public bool isAdmin { get; set; } = false;
        }
    }
}
