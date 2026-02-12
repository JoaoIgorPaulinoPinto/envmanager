using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace envmanager.src.data.schemes
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";
        [BsonElement("username")]
        public string UserName { get; set; } = "";
        [BsonElement("email")]
        public string Email { get; set; } = "";
        [BsonElement("password")]
        public string Password { get; set; } = "";
        [BsonElement("refresh_token")]
        public string RefreshToken { get; set; } = "";
    }
}
