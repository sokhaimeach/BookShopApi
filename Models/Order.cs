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

public class OrderDetail
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = String.Empty;
    [BsonElement("order_date")]
    public DateTime OrderDate { get; set; }
    [BsonElement("total")]
    public decimal Total { get; set; }
    [BsonElement("user_name")]
    public string? UserName { get; set; }
    [BsonElement("order_list_info")]
    public List<BookInfomation> OrderListInfo { get; set; } = new List<BookInfomation>();
}

public class BookInfomation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = String.Empty;
    [BsonElement("title")]
    public string? Title { get; set; }
    [BsonElement("price")]
    public decimal Price { get; set; }
    [BsonElement("quantity")]
    public int Quantity { get; set; }
}