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
    private readonly OrderService _orderService;
    // construtor
    public BookService(IOptions<DatabaseSettings> databaseSettings, OrderService orderService)
    {
        // initialize MongoDb client
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        // connect to database
        var database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        // connect to collection
        _bookCollection = database.GetCollection<Book>(databaseSettings.Value.BooksCollection);
        // order
        _orderService = orderService;
    }
    // get all books
    public async Task<List<BookWithAuthorName>> GetAllAsync()
    {
        BsonDocument lookupAuthor = new BsonDocument
        {
            {"$lookup", new BsonDocument
                {
                    {"from", "Authors"},
                    {"localField", "author_id"},
                    {"foreignField", "_id" },
                    {"as", "author_info"}
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
        BsonDocument addField = new BsonDocument
        {
            {"$addFields", new BsonDocument
                {
                    {"author_name", "$author_info.name"}
                }
            }
        };
        BsonDocument selectFields = new BsonDocument
        {
            {"$project", new BsonDocument
                {
                    {"title",1 },
                    {"price",1 },
                    {"category",1 },
                    {"author_id",1 },
                    {"image_url",1 },
                    {"description",1 },
                    {"publish_date",1 },
                    {"favorite",1 },
                    {"reviews",1 },
                    {"author_name",1 }
                }
            }
        };
        var pipeline = new[] { lookupAuthor, unwindAuthor, addField, selectFields };
        var result = await _bookCollection.Aggregate<BookWithAuthorName>(pipeline).ToListAsync();
        return result;
    }
    // add new book
    public async Task<string> AddAsync([FromBody] Book book)
    {
        book.PublishDate = DateTime.Now;
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
        BsonDocument addConvertedUserId = new BsonDocument {
            { "$addFields", new BsonDocument {
                { "reviews", new BsonDocument {
                    { "$map", new BsonDocument {
                        { "input", "$reviews" },
                        { "as", "r" },
                        { "in", new BsonDocument {
                            // only convert if user_id looks like a valid ObjectId
                            { "user_id", new BsonDocument {
                                { "$cond", new BsonArray {
                                    // condition: user_id matches 24 hex chars
                                    new BsonDocument("$regexMatch", new BsonDocument {
                                        { "input", "$$r.user_id" },
                                        { "regex", "^[0-9a-fA-F]{24}$" }
                                    }),
                                    new BsonDocument("$toObjectId", "$$r.user_id"),
                                    "$$r.user_id" // keep original (empty or invalid)
                                }}
                            }},
                            { "comment", "$$r.comment" },
                            { "rate", "$$r.rate" }
                        }}
                    }}
                }}
            }}
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
        var pipeline = new[] { filter, lookupAuthor, unwindAuthor, addConvertedUserId, lookupReviews, merge, filterFields };
        var result = await _bookCollection.Aggregate<BookDetail>(pipeline).FirstOrDefaultAsync();
        return result;
    }
    // get books by author id
    public async Task<List<Book>> GetBooksByAuthorId(string id)
    {
        var filter = Builders<Book>.Filter.Eq(a => a.AuthorId, id);
        var result = await _bookCollection.Find(filter).ToListAsync();
        return result;
    }
    // get books by category
    public async Task<List<Book>> GetBooksByCategory(string category)
    {
        var filter = Builders<Book>.Filter.Eq(a => a.Category, category);
        var books = await _bookCollection.Find(filter).ToListAsync();
        return books;
    }
    // find user's wishlist books
    public async Task<List<Book>> GetBookByWishList(string UserId)
    {
        BsonDocument filter = new BsonDocument
        {
            {"favorite", new BsonDocument
                {
                    {"$in", new BsonArray{UserId}}
                }
            }
        };
        var books = await _bookCollection.Find(filter).ToListAsync();
        return books;
    }
    // best seller
    public async Task<List<Book>> GetBestSellBooks()
    {
        var orders = await _orderService.GetAllAsync();

        var quantityByBook = new Dictionary<string, int>();

        foreach (var order in orders)
        {
            foreach (var info in order.OrderListInfo)
            {
                if (quantityByBook.ContainsKey(info.Id))
                    quantityByBook[info.Id] += info.Quantity;
                else
                    quantityByBook[info.Id] = info.Quantity;
            }
        }

        // Sort by quantity descending
        var sortedBookIds = quantityByBook
            .OrderByDescending(x => x.Value)
            .Select(x => x.Key)
            .ToList();

        // Find all books in MongoDB
        var filter = Builders<Book>.Filter.In(b => b.Id, sortedBookIds);
        var books = await _bookCollection.Find(filter).Limit(8).ToListAsync();

        // Optional: order the books according to their best-sell ranking
        books = books.OrderBy(b => sortedBookIds.IndexOf(b.Id)).ToList();

        return books;
    }
    // filter related product
    public async Task<List<Book>> GetRelatedProduct(string id, string category){
        // prevent the same product
        var prevent = Builders<Book>.Filter.Ne(a => a.Id, id);
        var sameCate = Builders<Book>.Filter.Eq(a => a.Category, category);
        var filter = Builders<Book>.Filter.And(prevent, sameCate);
        var books = await _bookCollection.Find(filter).ToListAsync();
        return books;
    }
}
