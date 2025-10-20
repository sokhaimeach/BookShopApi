using BookApi.Configuration;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookApi.Services;

public class OrderService
{
    private readonly IMongoCollection<Order> _orderCollection;
    public OrderService(IOptions<DatabaseSettings> setting)
    {
        // initialize Mongo client
        var mongoClient = new MongoClient(setting.Value.ConnectionString);
        // connet to database
        var database = mongoClient.GetDatabase(setting.Value.DatabaseName);
        // connect to collection
        _orderCollection = database.GetCollection<Order>(setting.Value.OrdersCollection);
    }
    // get all orders
    public async Task<List<Order>> GetAllAsync() =>
        await _orderCollection.Find(_ => true).ToListAsync();

    // insert to database
    public async Task<string> AddAsync([FromBody] Order order)
    {
        await _orderCollection.InsertOneAsync(order);
        return order.Id;
    } 
}