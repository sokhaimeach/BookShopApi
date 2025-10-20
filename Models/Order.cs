using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = String.Empty;
    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; set; }
    [BsonElement("order_date")]
    public DateTime OrderDate { get; set; }
    [BsonElement("total")]
    public decimal Total { get; set; }
    [BsonElement("order_list")]
    public List<OrderItem> OrderList { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    [BsonElement("book_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? BookId { get; set; }
    [BsonElement("quantity")]
    public int Quantity { get; set; }
}