using Azure.Storage.Blobs;
using ChatService.Domain.Abstractions.BlobService;
using ChatService.Domain.Abstractions.DbInitializer;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Infrastructure.Configurations;
using ChatService.Infrastructure.Repositories;
using ChatService.Infrastructure.Services.BlobService;
using ChatService.Infrastructure.Services.DbInitializer;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Serilog;
using Serilog.Formatting.Compact;

namespace ChatService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<MongoDbSettings>()
            .BindConfiguration("MongoDbSettings");

        services.AddOptionsWithValidateOnStart<AzuriteSettings>()
            .BindConfiguration("AzuriteSettings");
        
        var mongoSettings = configuration.GetRequiredSection("MongoDbSettings").Get<MongoDbSettings>()!;
        var azuriteSettings = configuration.GetRequiredSection("AzuriteSettings").Get<AzuriteSettings>()!;
        
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

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
        
        services.AddScoped<IUnitOfWork, AppUnitOfWork>();
        services.AddScoped<IDbInitializer, DbInitializer>();
        
        services.AddHealthChecks()
            .AddMongoDb(_ => new MongoClient(mongoSettings.ConnectionString))
            .AddAzureBlobStorage(_ => new BlobServiceClient(azuriteSettings.ConnectionString));
        
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", "ChatService")
            .WriteTo.Console(new CompactJsonFormatter())
            .WriteTo.Http(
                requestUri: configuration["Logstash:Url"]!,
                queueLimitBytes: null,
                textFormatter: new CompactJsonFormatter())
            .CreateLogger();

        services.AddLogging(logging => logging.AddSerilog());
        
        return services;
    }
}