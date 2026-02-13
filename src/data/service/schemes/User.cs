using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.schemes
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        [MinLength(1)]
        [BsonElement("username")]
        [MaxLength(255)]
        public string UserName { get; set; } = "";
        [MaxLength(255)]
        [EmailAddress]
        [BsonElement("email")]
        public string Email { get; set; } = "";
        [PasswordPropertyText]
        [BsonElement("password")]
        [MaxLength(32)]
        [MinLength(6)]
        public string Password { get; set; } = "";



        [BsonElement("refresh_token")]
        public string RefreshToken { get; set; } = "";
        [BsonElement("refresh_token_expiry")]
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
