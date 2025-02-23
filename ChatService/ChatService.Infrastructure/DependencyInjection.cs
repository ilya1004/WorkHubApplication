using ChatService.Infrastructure.Configurations;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace ChatService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        
        var mongoSettings = configuration.GetRequiredSection("MongoDbSettings").Get<MongoDbSettings>()!;
        
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoSettings.DatabaseName);
        });
        
        ChatConfiguration.Configure();
        MessageConfiguration.Configure();

        return services;
    }
}