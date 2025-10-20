using BookApi.Configuration;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookApi.Services;

public class BookService
{
    private readonly IMongoCollection<Book> _bookCollection;
    // construtor
    public BookService(IOptions<DatabaseSettings> databaseSettings)
    {
        // initialize MongoDb client
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        // connect to database
        var database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        // connect to collection
        _bookCollection = database.GetCollection<Book>(databaseSettings.Value.BooksCollection);
    }
    // get all books
    public async Task<List<Book>> GetAllAsync() =>
        await _bookCollection.Find(_ => true).ToListAsync();
    // add new book
    public async Task<string> AddAsync([FromBody] Book book)
    {
        book.PublishDate = DateTime.UtcNow;
        book.PublishDate.ToString("yyyy-MM-dd HH:mm:ss");
        await _bookCollection.InsertOneAsync(book);
        return book.Id;
    }
    // delete book
    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<Book>.Filter.Eq(a => a.Id, id);
        var result = await _bookCollection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
    // update book
    public async Task<bool> UpdateAsync([FromBody] Book book)
    {
        var filter = Builders<Book>.Filter.Eq(a => a.Id, book.Id);
        var result = await _bookCollection.ReplaceOneAsync(filter, book);
        return result.ModifiedCount == 1;
    }
    // get one book by id
    public async Task<BookDetail> GetOneAsync(string id)
    {
        BsonDocument filter = new BsonDocument
        {
            { "$match", new BsonDocument
                {
                    { "_id", ObjectId.Parse(id) }
                }
            }
        };
        BsonDocument lookupAuthor = new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "Authors" },
                    { "localField", "author_id" },
                    { "foreignField", "_id" },
                    { "as", "author_info" }
                }
            }
        };
        BsonDocument unwindAuthor = new BsonDocument
        {
            {"$unwind", new BsonDocument
                {
                    {"path", "$author_info"},
                    {"preserveNullAndEmptyArrays", true}
                }
            }
        };
        BsonDocument lookupReviews = new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "Users" },
                    { "localField", "reviews.user_id" },
                    { "foreignField", "_id" },
                    { "as", "reviewer_info" }
                }
            }
        };
        BsonDocument merge = new BsonDocument
        {
            { "$addFields", new BsonDocument
                {
                    { "reviewer_info", new BsonDocument
                        {
                            { "$map", new BsonDocument
                                {
                                    { "input", "$reviews" },
                                    { "as", "review" },
                                    { "in", new BsonDocument
                                        {
                                            { "$mergeObjects", new BsonArray
                                            {
                                                new BsonDocument{
                                                    {
                                                    "$arrayElemAt",
                                                    new BsonArray
                                                    {
                                                        new BsonDocument
                                                        {
                                                            { "$filter", new BsonDocument
                                                                {
                                                                    { "input", "$reviewer_info" },
                                                                    { "as", "u" },
                                                                    { "cond", new BsonDocument
                                                                        {
                                                                            { "$eq", new BsonArray { "$$u._id", "$$review.user_id" } }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }, 0
                                                    }
                                                    }
                                                },
                                                    new BsonDocument
                                                    {
                                                        { "comment", "$$review.comment" },
                                                        { "rate", "$$review.rate" }
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
        BsonDocument filterFields = new BsonDocument
        {
            { "$project", new BsonDocument
                {
                    { "title", 1},
                    { "price", 1},
                    { "category", 1},
                    { "image_url", 1},
                    { "description", 1},
                    { "publish_date", 1},
                    { "favorite", 1},
                    { "author_info", 1 },
                    { "reviewer_info", new BsonDocument
                        {
                            { "_id", 1 },
                            { "name", 1 },
                            { "image_url", 1 },
                            { "comment", 1 },
                            { "rate", 1 }
                        }
                    }
                }
            }
        };
        var pipeline = new[] { filter, lookupAuthor, lookupReviews, merge, unwindAuthor, filterFields };
        var result = await _bookCollection.Aggregate<BookDetail>(pipeline).FirstOrDefaultAsync();
        return result;
    }
}