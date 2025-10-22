using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.Models;

public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = String.Empty;
    [BsonElement("title")]
    public string? Title { get; set; }
    [BsonElement("price")]
    public decimal Price { get; set; }
    [BsonElement("category")]
    public string? Category { get; set; }
    [BsonElement("author_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string AuthorId { get; set; } = String.Empty;
    [BsonElement("image_url")]
    public string? ImageUrl { get; set; }
    [BsonElement("description")]
    public string? Description { get; set; }
    [BsonElement("publish_date")]
    public DateTime PublishDate { get; set; }
    [BsonElement("favorite")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Favorite { get; set; } = new List<string>();
    [BsonElement("reviews")]
    public List<Review> Reviews { get; set; } = new List<Review>();
}

public class BookWithAuthorName:Book
{
    [BsonElement("author_name")]
    public string AuthorName { get; set; } = String.Empty;
}

public class Review
{
    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = String.Empty;
    [BsonElement("comment")]
    public string? Comment { get; set; }
    [BsonElement("rate")]
    public int? Rate { get; set; }
}

public class BookDetail
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = String.Empty;
    [BsonElement("title")]
    public string? Title { get; set; }
    [BsonElement("price")]
    public decimal Price { get; set; }
    [BsonElement("category")]
    public string? Category { get; set; }
    [BsonElement("image_url")]
    public string? ImageUrl { get; set; }
    [BsonElement("description")]
    public string? Description { get; set; }
    [BsonElement("publish_date")]
    public DateTime PublishDate { get; set; }
    [BsonElement("favorite")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Favorite { get; set; } = new List<string>();
    [BsonElement("author_info")]
    public Author AuthorInfo { get; set; } = new Author();
    [BsonElement("reviewer_info")]
    public List<Reviewer> ReviewerInfo { get; set; } = new List<Reviewer>();
}

public class Reviewer
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = String.Empty;
    [BsonElement("name")]
    public string? Name { get; set; }
    [BsonElement("image_url")]
    public string? ImageUrl { get; set; }
    [BsonElement("comment")]
    public string? Comment { get; set; }
    [BsonElement("rate")]
    public int? Rate { get; set; }
}