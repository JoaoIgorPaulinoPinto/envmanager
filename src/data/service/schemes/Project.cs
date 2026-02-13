using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.schemes
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        [BsonElement("user_id")]
        public string UserId { get; set; } = "";
        [BsonElement("project_name")]
        [MinLength(1), MaxLength(255)]
        public string ProjectName { get; set; } = "";
        [BsonElement("description")]
        [MaxLength(255)]
        public string Description { get; set; } = "";
        [BsonElement("password")]
        [PasswordPropertyText]
        [MinLength(6), MaxLength(255)] 
        public string? Password { get; set; } = "";

        [BsonElement("variables")]
        public List<Key> Variables { get; set; } = new List<Key>()  ;
    }
}