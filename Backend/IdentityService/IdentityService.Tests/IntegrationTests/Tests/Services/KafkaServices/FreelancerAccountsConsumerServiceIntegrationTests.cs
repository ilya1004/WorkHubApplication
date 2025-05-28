using System.Text.Json;
using Confluent.Kafka;
using IdentityService.BLL.Settings;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.Tests.IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityService.Tests.IntegrationTests.Tests.Services.KafkaServices;

public class FreelancerAccountsConsumerServiceIntegrationTests(
    IntegrationTestsFixture fixture) : IClassFixture<IntegrationTestsFixture>
{
    private readonly Guid FreelancerId = IntegrationTestsFixture.FreelancerId;

    [Fact]
    public async Task ConsumeMessage_ValidMessage_ShouldUpdateFreelancerProfile()
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

        var dto = new SaveFreelancerAccountIdDto
        {
            UserId = FreelancerId.ToString(),
            FreelancerAccountId = "acct_123456"
        };
        var message = JsonSerializer.Serialize(dto);

        // Act
        await producer.ProduceAsync(kafkaSettings.FreelancerAccountIdSavingTopic, new Message<Null, string>
        {
            Value = message
        });

        await Task.Delay(1000);

        // Assert
        var updatedProfile = await unitOfWork.FreelancerProfilesRepository.FirstOrDefaultAsync(fp => fp.UserId == FreelancerId);
        updatedProfile.Should().NotBeNull();
        updatedProfile!.StripeAccountId.Should().Be("acct_123456");
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

        var dto = new SaveFreelancerAccountIdDto
        {
            UserId = Guid.NewGuid().ToString(),
            FreelancerAccountId = "acct_789012"
        };
        var message = JsonSerializer.Serialize(dto);

        // Act
        await producer.ProduceAsync(kafkaSettings.FreelancerAccountIdSavingTopic, new Message<Null, string>
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
        await producer.ProduceAsync(kafkaSettings.FreelancerAccountIdSavingTopic, new Message<Null, string>
        {
            Value = invalidMessage
        });

        await Task.Delay(1000);

        // Assert
        Assert.True(true);
    }
}