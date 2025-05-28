using Azure.Storage.Blobs;
using ChatService.Domain.Abstractions.AzuriteStartupService;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Infrastructure.Configurations;
using ChatService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.Azurite;
using Testcontainers.MongoDb;

namespace ChatService.Tests.IntegrationTests.Helpers;

public class IntegrationTestsFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer;
    private readonly AzuriteContainer _azuriteContainer;
    internal WebApplicationFactory<Program> Factory { get; set; }

    public IntegrationTestsFixture()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .WithUsername("root")
            .WithPassword("mongopass")
            .WithPortBinding(27017, true)
            .Build();

        _azuriteContainer = new AzuriteBuilder()
            .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
            .WithPortBinding(10000, 10000)
            .WithPortBinding(10001, 10001)
            .WithPortBinding(10002, 10002)
            .Build();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IMongoClient>(sp =>
                {
                    var connectionString = _mongoDbContainer.GetConnectionString();
                    return new MongoClient(connectionString);
                });

                services.AddSingleton<IMongoDatabase>(sp =>
                {
                    var client = sp.GetRequiredService<IMongoClient>();
                    return client.GetDatabase("ChatServiceDb");
                });

                services.AddSingleton(_ => new BlobServiceClient(
                    $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint={_azuriteContainer.GetBlobEndpoint()};"
                ));

                services.AddScoped<IUnitOfWork, AppUnitOfWork>();
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "MongoDbSettings:ConnectionString", _mongoDbContainer.GetConnectionString() },
                    { "MongoDbSettings:DatabaseName", "ChatServiceDb" },
                    {
                        "AzuriteSettings:ConnectionString",
                        $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint={_azuriteContainer.GetBlobEndpoint()};"
                    },
                    { "AzuriteSettings:FilesContainerName", "chat-files" }
                });
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
        await _azuriteContainer.StartAsync();

        using var scope = Factory.Services.CreateScope();
        var azuriteService = scope.ServiceProvider.GetRequiredService<IAzuriteStartupService>();
        await azuriteService.CreateContainerIfNotExistAsync();

        ChatConfiguration.Configure();
        MessageConfiguration.Configure();
    }

    public async Task DisposeAsync()
    {
        await _mongoDbContainer.StopAsync();
        await _mongoDbContainer.DisposeAsync();
        await _azuriteContainer.StopAsync();
        await _azuriteContainer.DisposeAsync();
        await Factory.DisposeAsync();
    }
}