using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = String.Empty;
    [BsonElement("name")]
    public string? Name { get; set; }
    [BsonElement("image_url")]
    public string? ImageUrl { get; set; }
    [BsonElement("password")]
    public string Password { get; set; } = String.Empty;
    [BsonElement("role")]
    public string Role { get; set; } = String.Empty;
    [BsonElement("email")]
    public string? Email { get; set; }
    [BsonElement("description")]
    public string? Description { get; set; }
}
