using Confluent.Kafka;
using Confluent.Kafka.Admin;
using IdentityService.BLL.Abstractions.AzuriteStartupService;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.Azurite;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace IdentityService.Tests.IntegrationTests.Helpers;

public class IntegrationTestsFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private readonly RedisContainer _redisContainer;
    private readonly AzuriteContainer _azuriteContainer;
    private readonly KafkaContainer _kafkaContainer;
    internal WebApplicationFactory<Program> Factory { get; set; }

    public const string AdminRole = "Admin";
    public const string EmployerRole = "Employer";
    public const string FreelancerRole = "Freelancer";
    public static readonly Guid EmployerId = Guid.Parse("e13341b4-6532-41f6-9595-202525c7ff34");
    public static readonly Guid FreelancerId = Guid.Parse("52d78d21-8f4d-469d-a911-b094d6f9994b");

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

        _azuriteContainer = new AzuriteBuilder()
            .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
            .WithPortBinding(10000, 10000)
            .WithPortBinding(10001, 10001)
            .WithPortBinding(10002, 10002)
            .Build();

        _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:latest")
            .WithPortBinding(9092, 9092)
            .WithEnvironment("KAFKA_AUTO_CREATE_TOPICS_ENABLE", "true")
            .Build();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(_postgreSqlContainer.GetConnectionString()));

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = _redisContainer.GetConnectionString();
                });
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:PostgresConnection", _postgreSqlContainer.GetConnectionString() },
                    { "ConnectionStrings:RedisConnection", _redisContainer.GetConnectionString() },
                    {
                        "ConnectionStrings:AzuriteConnection",
                        $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint={_azuriteContainer.GetBlobEndpoint()};"
                    },
                    { "CacheOptions:RecordExpirationTimeInMinutes", "5" },
                    { "Azurite:ImagesContainerName", "user-images" },
                    { "KafkaSettings:BootstrapServers", _kafkaContainer.GetBootstrapAddress() },
                    { "KafkaSettings:EmployerAccountIdSavingTopic", "employer_account_id_saving" },
                    { "KafkaSettings:FreelancerAccountIdSavingTopic", "freelancer_account_id_saving" }
                });
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _azuriteContainer.StartAsync();
        await _kafkaContainer.StartAsync();
        
        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _kafkaContainer.GetBootstrapAddress()
        }).Build();

        try
        {
            await adminClient.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = "employer_account_id_saving",
                    NumPartitions = 1,
                    ReplicationFactor = 1
                },
                new TopicSpecification
                {
                    Name = "freelancer_account_id_saving",
                    NumPartitions = 1,
                    ReplicationFactor = 1
                }
            });
            Console.WriteLine("Kafka topics created successfully.");
        }
        catch (CreateTopicsException ex)
        {
            Console.WriteLine($"Failed to create topics: {ex.Message}");
            throw;
        }

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        
        await dbContext.Database.MigrateAsync();
        
        var azuriteService = scope.ServiceProvider.GetRequiredService<IAzuriteStartupService>();
        await azuriteService.CreateContainerIfNotExistAsync();
        
        if (await roleManager.FindByNameAsync(AdminRole) == null)
        {
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = AdminRole, NormalizedName = AdminRole.ToUpper() });
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = EmployerRole, NormalizedName = EmployerRole.ToUpper() });
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = FreelancerRole, NormalizedName = FreelancerRole.ToUpper() });
        }
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
        await _redisContainer.StopAsync();
        await _redisContainer.DisposeAsync();
        await _azuriteContainer.StopAsync();
        await _azuriteContainer.DisposeAsync();
        await _kafkaContainer.StopAsync();
        await _kafkaContainer.DisposeAsync();
        await Factory.DisposeAsync();
    }
}