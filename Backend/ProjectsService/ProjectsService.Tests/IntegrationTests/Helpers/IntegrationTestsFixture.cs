using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectsService.Infrastructure.Data;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace ProjectsService.Tests.IntegrationTests.Helpers;

public class IntegrationTestsFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private readonly RedisContainer _redisContainer;
    private readonly KafkaContainer _kafkaContainer;
    internal WebApplicationFactory<Program> Factory { get; private set; }

    public IntegrationTestsFixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("test_db")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

        _redisContainer = new RedisBuilder()
            .WithImage("redis:latest")
            .WithPortBinding(6379, true)
            .Build();

        _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:latest")
            .WithPortBinding(9092, 9092)
            .Build();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CommandsDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<QueriesDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IHostedService));
                while (descriptor != null)
                {
                    services.Remove(descriptor);
                    descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IHostedService));
                }

                var postgresConnectionString = _postgreSqlContainer.GetConnectionString();
                services.AddDbContext<CommandsDbContext>(options =>
                    options.UseNpgsql(postgresConnectionString));
                services.AddDbContext<QueriesDbContext>(options =>
                    options.UseNpgsql(postgresConnectionString));

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = _redisContainer.GetConnectionString();
                });
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:PostgresConnectionPrimaryDb", _postgreSqlContainer.GetConnectionString() },
                    { "ConnectionStrings:PostgresConnectionReplicaDb", _postgreSqlContainer.GetConnectionString() },
                    { "ConnectionStrings:PostgresConnectionHangfireDb", _postgreSqlContainer.GetConnectionString() },
                    { "ConnectionStrings:RedisConnection", _redisContainer.GetConnectionString() },
                    { "KafkaSettings:BootstrapServers", _kafkaContainer.GetBootstrapAddress() },
                    { "KafkaSettings:PaymentIntentSavingTopic", "payment_intent_saving" },
                    { "KafkaSettings:PaymentCancellationTopic", "payment_cancellation" }
                });
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _kafkaContainer.StartAsync();
        using var scope = Factory.Services.CreateScope();
        var commandsDbContext = scope.ServiceProvider.GetRequiredService<CommandsDbContext>();
        await commandsDbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
        await _redisContainer.StopAsync();
        await _redisContainer.DisposeAsync();
        await _kafkaContainer.StopAsync();
        await _kafkaContainer.DisposeAsync();
        await Factory.DisposeAsync();
    }
}