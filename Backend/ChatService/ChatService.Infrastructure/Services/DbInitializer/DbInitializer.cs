using ChatService.Domain.Abstractions.DbInitializer;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatService.Infrastructure.Services.DbInitializer;

public class DbInitializer(
    IOptions<MongoDbSettings> options,
    ILogger<DbInitializer> logger) : IDbInitializer
{
    public async Task InitializeDbAsync(IConfiguration configuration)
    {
        logger.LogInformation("Starting database initialization");

        try
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            
            logger.LogInformation("Connected to MongoDB database: {DatabaseName}", options.Value.DatabaseName);

            var collections = (await database.ListCollectionNamesAsync()).ToList();

            if (!collections.Contains("Chats"))
            {
                logger.LogInformation("Creating 'Chats' collection");
                
                await database.CreateCollectionAsync("Chats");
            }

            if (!collections.Contains("Messages"))
            {
                logger.LogInformation("Creating 'Messages' collection");
                
                await database.CreateCollectionAsync("Messages");
            }

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialization failed");
            throw new Exception("Database initialization failed", ex);
        }
    }
}