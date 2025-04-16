using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProjectsService.Domain.Abstractions.KafkaProducerServices;
using ProjectsService.Infrastructure.Settings;
using ProjectsService.Tests.IntegrationTests.Helpers;

namespace ProjectsService.Tests.IntegrationTests.Tests.KafkaServices;

public class PaymentsProducerServiceIntegrationTests(
    IntegrationTestsFixture fixture) : IClassFixture<IntegrationTestsFixture>
{
    [Fact]
    public async Task CancelPaymentAsync_ValidPaymentId_ShouldProduceMessage()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var producerService = scope.ServiceProvider.GetRequiredService<IPaymentsProducerService>();
        var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;
        
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers
        };
        using var adminClient = new AdminClientBuilder(adminConfig).Build();
        try
        {
            await adminClient.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = kafkaSettings.PaymentCancellationTopic,
                    NumPartitions = 1,
                    ReplicationFactor = 1
                }
            });
        }
        catch (CreateTopicsException e) when (e.Results.Any(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
        {
            
        }
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            GroupId = "test_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(kafkaSettings.PaymentCancellationTopic);

        var paymentId = "pi_123456";

        // Act
        await producerService.CancelPaymentAsync(paymentId, It.IsAny<CancellationToken>());
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var consumeResult = consumer.Consume(cts.Token);

        // Assert
        Assert.NotNull(consumeResult);
        Assert.Equal(paymentId, consumeResult.Message.Value);
    }
}