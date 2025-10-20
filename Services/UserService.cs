using BookApi.Configuration;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookApi.Services;

public class UserService
{
    private readonly IMongoCollection<User> _userCollection;
    // construtor
    public UserService(IOptions<DatabaseSettings> databaseSettings)
    {
        // initialize MongoDb client
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        // connect to database
        var database = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        // connect to collection
        _userCollection = database.GetCollection<User>(databaseSettings.Value.UsersCollection);
    }
    // get all users
    public async Task<List<User>> GetAllAsync() =>
        await _userCollection.Find(_ => true).ToListAsync();
    // add new user
    public async Task<string> AddAsync([FromBody] User user)
    {
        await _userCollection.InsertOneAsync(user);
        return user.Id;
    }
    // delete user
    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq(a => a.Id, id);
        var result = await _userCollection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
    // update user
    public async Task<bool> UpdateAsync([FromBody] User user)
    {
        var filter = Builders<User>.Filter.Eq(a => a.Id, user.Id);
        var result = await _userCollection.ReplaceOneAsync(filter, user);
        return result.ModifiedCount == 1;
    }
}