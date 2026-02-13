using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.schemes
{
    public class ProjectMember
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        [Required]
        [BsonElement("is_admin")]
        public bool isAdmin { get; set; } = false;
    }
}
