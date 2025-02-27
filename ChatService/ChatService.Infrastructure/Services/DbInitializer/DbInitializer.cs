using ChatService.Domain.Abstractions.DbInitializer;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Services.DbInitializer;

public class DbInitializer : IDbInitializer
{
    public async Task InitializeDbAsync(IConfiguration configuration)
    {
        var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()!;

        var client = new MongoClient(mongoDbSettings.ConnectionString);
        var database = client.GetDatabase(mongoDbSettings.DatabaseName);
        
        var collections = (await database.ListCollectionNamesAsync()).ToList();
        
        if (!collections.Contains("Chats"))
        {
            await database.CreateCollectionAsync("Chats");
        }

        if (!collections.Contains("Messages"))
        {
            await database.CreateCollectionAsync("Messages");
        }
    }
}