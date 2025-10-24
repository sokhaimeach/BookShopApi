using BookApi.Configuration;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
    public async Task<List<OrderDetail>> GetAllAsync()
    {
        BsonDocument lookupUser = new BsonDocument
        {
            {"$lookup", new BsonDocument
                {
                    {"from", "Users"},
                    {"localField", "user_id" },
                    {"foreignField", "_id" },
                    {"as", "user_info"}
                }
            }
        };
        BsonDocument unwindUser = new BsonDocument
        {
            {"$unwind", new BsonDocument
                {
                    {"path", "$user_info"},
                    {"preserveNullAndEmptyArrays", true}
                }
            }
        };
        BsonDocument lookupBook = new BsonDocument
        {
            {"$lookup", new BsonDocument
                {
                    {"from", "Books"},
                    {"localField", "order_list.book_id"},
                    {"foreignField", "_id"},
                    {"as", "book_info"}
                }
            }
        };
        var addFieldBook = new BsonDocument
{
    {
        "$addFields", new BsonDocument
        {
            {
                "order_list_info", new BsonDocument
                {
                    {
                        "$map", new BsonDocument
                        {
                            { "input", "$order_list" },
                            { "as", "item" },
                            { "in", new BsonDocument
                                {
                                    { "_id", "$$item.book_id" },
                                    { "quantity", "$$item.quantity" },
                                    { "title", new BsonDocument
                                        {
                                            { "$let", new BsonDocument
                                                {
                                                    { "vars", new BsonDocument
                                                        {
                                                            { "book", new BsonDocument
                                                                {
                                                                    { "$arrayElemAt", new BsonArray
                                                                        {
                                                                            new BsonDocument
                                                                            {
                                                                                { "$filter", new BsonDocument
                                                                                    {
                                                                                        { "input", "$book_info" },
                                                                                        { "as", "b" },
                                                                                        { "cond", new BsonDocument
                                                                                            {
                                                                                                { "$eq", new BsonArray { "$$b._id", "$$item.book_id" } }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            },
                                                                            0
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    },
                                                    { "in", "$$book.title" }
                                                }
                                            }
                                        }
                                    },
                                    { "price", new BsonDocument
                                        {
                                            { "$let", new BsonDocument
                                                {
                                                    { "vars", new BsonDocument
                                                        {
                                                            { "book", new BsonDocument
                                                                {
                                                                    { "$arrayElemAt", new BsonArray
                                                                        {
                                                                            new BsonDocument
                                                                            {
                                                                                { "$filter", new BsonDocument
                                                                                    {
                                                                                        { "input", "$book_info" },
                                                                                        { "as", "b" },
                                                                                        { "cond", new BsonDocument
                                                                                            {
                                                                                                { "$eq", new BsonArray { "$$b._id", "$$item.book_id" } }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            },
                                                                            0
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    },
                                                    { "in", "$$book.price" }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
};
        BsonDocument project = new BsonDocument
        {
            {"$project", new BsonDocument
                {
                    { "_id", 1 },
                    { "order_date", 1 },
                    { "total", 1 },
                    {"user_name", "$user_info.name"},
                    {"order_list_info",1}
                }
            }
        };
        var pipeline = new[] { lookupUser, unwindUser, lookupBook, addFieldBook, project };
        var result = await _orderCollection.Aggregate<OrderDetail>(pipeline).ToListAsync();
        return result;
    }

    // insert to database
    public async Task<string> AddAsync([FromBody] Order order)
    {
        await _orderCollection.InsertOneAsync(order);
        return order.Id;
    }
}