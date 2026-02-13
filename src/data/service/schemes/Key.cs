using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.schemes
{
    public class Key
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        [MinLength(1)]
        [BsonElement("key")]
        [MaxLength(255)]
        public string ProjecetName { get; set; } = "";
        [MaxLength(255)]
        [EmailAddress]
        [BsonElement("value")]
        public string Value { get; set; } = "";
    }
}
