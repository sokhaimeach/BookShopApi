using BookApi.Configuration;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookApi.Services;

public class AuthorService
{
    private readonly IMongoCollection<Author> _authorsCollection;
    // constructor
    public AuthorService(IOptions<DatabaseSettings> databaseSettings)
    {
        // initialize MongoDB client
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        // connect to the database
        var database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        // connect to the authors collection
        _authorsCollection = database.GetCollection<Author>(databaseSettings.Value.AuthorsCollection);
    }
    // get all authors
    public async Task<List<Author>> GetAllAsync() =>
        await _authorsCollection.Find(_ => true).ToListAsync();

    // add author
    public async Task<string> AddAsync([FromBody] Author author)
    {
        await _authorsCollection.InsertOneAsync(author);
        return author.Id;
    }
    // delete author
    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<Author>.Filter.Eq(a => a.Id, id);
        var result = await _authorsCollection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
    // update author
    public async Task<bool> UpdateAsync([FromBody] Author author)
    {
        var filter = Builders<Author>.Filter.Eq(a => a.Id, author.Id);
        await _authorsCollection.ReplaceOneAsync(filter, author);
        return true;
    }
}