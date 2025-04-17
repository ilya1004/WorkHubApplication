using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.Kafka;

namespace PaymentsService.Tests.IntegrationTests.Helpers;

public class KafkaIntegrationTestsFixture : IAsyncLifetime
{
    private readonly KafkaContainer _kafkaContainer;
    internal WebApplicationFactory<Program> Factory { get; private set; }

    public KafkaIntegrationTestsFixture()
    {
        _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:latest")
            .WithPortBinding(9092, 9092)
            .WithEnvironment("KAFKA_AUTO_CREATE_TOPICS_ENABLE", "true")
            .Build();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IHostedService));
                while (descriptor != null)
                {
                    services.Remove(descriptor);
                    descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IHostedService));
                }
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "KafkaSettings:BootstrapServers", _kafkaContainer.GetBootstrapAddress() },
                    { "KafkaSettings:EmployerAccountIdSavingTopic", "employer_account_id_saving" },
                    { "KafkaSettings:FreelancerAccountIdSavingTopic", "freelancer_account_id_saving" },
                    { "KafkaSettings:PaymentIntentSavingTopic", "payment_intent_saving" },
                    { "KafkaSettings:PaymentCancellationTopic", "payment_cancellation" }
                });
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _kafkaContainer.StartAsync();
        
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = _kafkaContainer.GetBootstrapAddress()
        };
        using var adminClient = new AdminClientBuilder(adminConfig).Build();
        try
        {
            await adminClient.CreateTopicsAsync(new[]
            {
                new TopicSpecification { Name = "employer_account_id_saving", NumPartitions = 1, ReplicationFactor = 1 },
                new TopicSpecification { Name = "freelancer_account_id_saving", NumPartitions = 1, ReplicationFactor = 1 },
                new TopicSpecification { Name = "payment_intent_saving", NumPartitions = 1, ReplicationFactor = 1 },
                new TopicSpecification { Name = "payment_cancellation", NumPartitions = 1, ReplicationFactor = 1 }
            });
        }
        catch (CreateTopicsException e) when (e.Results.Any(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
        {
            
        }
    }

    public async Task DisposeAsync()
    {
        await _kafkaContainer.StopAsync();
        await _kafkaContainer.DisposeAsync();
        await Factory.DisposeAsync();
    }
}