﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace envmanager.src.data.service.schemes
{
    public class Key
    {
        [Required(ErrorMessage ="Id not provided")]
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; } = Guid.NewGuid().ToString();
        [MinLength(1)]
        [BsonElement("variable")]
        [Required(ErrorMessage = "Variable not provided")]

        [MaxLength(255)]
        public string Variable { get; set; } = "";
        [MaxLength(255)]
        [Required(ErrorMessage = "Value not provided")]
        [EmailAddress]
        [BsonElement("value")]
        public string Value { get; set; } = "";
    }
}