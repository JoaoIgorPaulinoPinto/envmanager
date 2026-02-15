using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.schemes
{
    public class Invite
    {
        [Required(ErrorMessage = "Id not provided")]
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; } = "";
        [MinLength(1)]
        [BsonElement("invited_user_id")]
        [Required(ErrorMessage = "invited user not provided")]
        [MaxLength(255)]
        public string InvitedUserId { get; set; } = "";
        [MaxLength(255)]
        [Required(ErrorMessage = "inviter user not provided")]
        [BsonElement("inviter_user_id")]
        public string InviterUserId { get; set; } = "";
        
        [MaxLength(255)]
        [Required(ErrorMessage = "project not provided")]
        [BsonElement("project_id")]
        public string ProjectId { get; set; } = "";
    
        public DateTime? CreatedAt { get; set; }
    }
}
