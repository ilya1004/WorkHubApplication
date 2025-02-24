using Azure.Storage.Blobs;
using ChatService.Domain.Abstractions.BlobService;
using ChatService.Infrastructure.Configurations;
using ChatService.Infrastructure.Services.BlobService;
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
        services.Configure<AzuriteSettings>(configuration.GetSection("AzuriteSettings"));
        
        var mongoSettings = configuration.GetRequiredSection("MongoDbSettings").Get<MongoDbSettings>()!;
        var azuriteSettings = configuration.GetRequiredSection("AzuriteSettings").Get<AzuriteSettings>()!;
        
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoSettings.DatabaseName);
        });
        
        ChatConfiguration.Configure();
        MessageConfiguration.Configure();
        
        services.AddSingleton<IBlobService, BlobService>();
        services.AddSingleton(_ => new BlobServiceClient(azuriteSettings.ConnectionString));

        return services;
    }
}