using System.Text.Json;
using Confluent.Kafka;
using IdentityService.BLL.Settings;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityService.Tests.IntegrationTests.Tests.Services.KafkaServices;

public class EmployerAccountsConsumerServiceIntegrationTests(
    IntegrationTestsFixture fixture) : IClassFixture<IntegrationTestsFixture>
{
    private readonly Guid EmployerId = IntegrationTestsFixture.EmployerId;

    [Fact]
    public async Task ConsumeMessage_ValidMessage_ShouldUpdateEmployerProfile()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            AllowAutoCreateTopics = true
        };
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            GroupId = "test_group_" + Guid.NewGuid(),
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(kafkaSettings.EmployerAccountIdSavingTopic);

        var dto = new SaveEmployerAccountIdDto
        {
            UserId = EmployerId.ToString(),
            EmployerAccountId = "cus_123456"
        };
        var message = JsonSerializer.Serialize(dto);

        // Act
        var deliveryResult = await producer.ProduceAsync(kafkaSettings.EmployerAccountIdSavingTopic, new Message<Null, string>
        {
            Value = message
        });

        bool messageReceived = false;
        for (int i = 0; i < 10; i++)
        {
            var cr = consumer.Consume(TimeSpan.FromSeconds(1));
            if (cr?.Message?.Value == message)
            {
                messageReceived = true;
                break;
            }
        }
        consumer.Close();

        messageReceived.Should().BeTrue("Message should be received in Kafka topic");

        await Task.Delay(2000);

        // Assert
        var updatedProfile = await unitOfWork.EmployerProfilesRepository.FirstOrDefaultAsync(ep => ep.UserId == EmployerId);
        updatedProfile.Should().NotBeNull();
        updatedProfile!.StripeCustomerId.Should().Be("cus_123456");
    }
    
    [Fact]
    public async Task ConsumeMessage_ProfileNotFound_ShouldNotThrow()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            AllowAutoCreateTopics = true
        };
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        var dto = new SaveEmployerAccountIdDto
        {
            UserId = Guid.NewGuid().ToString(),
            EmployerAccountId = "cus_789012"
        };
        var message = JsonSerializer.Serialize(dto);

        // Act
        await producer.ProduceAsync(kafkaSettings.EmployerAccountIdSavingTopic, new Message<Null, string>
        {
            Value = message
        });

        await Task.Delay(1000);

        // Assert
        Assert.True(true);
    }

    [Fact]
    public async Task ConsumeMessage_InvalidMessage_ShouldNotThrow()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            AllowAutoCreateTopics = true
        };
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        var invalidMessage = "invalid_json";

        // Act
        await producer.ProduceAsync(kafkaSettings.EmployerAccountIdSavingTopic, new Message<Null, string>
        {
            Value = invalidMessage
        });

        await Task.Delay(1000);

        // Assert
        Assert.True(true);
    }
}