using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.Models;

public class Author{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = String.Empty;
    [BsonElement("name")]
    public string Name { get; set; } = String.Empty;
    [BsonElement("image_url")]
    public string? ImageUrl { get; set; }
    [BsonElement("bio")]
    public string? Bio { get; set; }
}
