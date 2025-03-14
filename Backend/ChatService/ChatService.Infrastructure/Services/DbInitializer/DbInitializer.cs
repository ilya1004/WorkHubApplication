using ChatService.Domain.Abstractions.DbInitializer;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ChatService.Infrastructure.Services.DbInitializer;

public class DbInitializer(IOptions<MongoDbSettings> options) : IDbInitializer
{
    public async Task InitializeDbAsync(IConfiguration configuration)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        
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