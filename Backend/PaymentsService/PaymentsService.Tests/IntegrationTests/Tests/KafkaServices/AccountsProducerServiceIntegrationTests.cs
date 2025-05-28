using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.Settings;
using PaymentsService.Tests.IntegrationTests.Helpers;

namespace PaymentsService.Tests.IntegrationTests.Tests.KafkaServices;

public class AccountsProducerServiceIntegrationTests(
    KafkaIntegrationTestsFixture fixture) : IClassFixture<KafkaIntegrationTestsFixture>
{
    [Fact]
    public async Task SaveEmployerAccountIdAsync_ValidData_ShouldProduceMessage()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var producerService = scope.ServiceProvider.GetRequiredService<IAccountsProducerService>();
        var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            GroupId = "test_group_employer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(kafkaSettings.EmployerAccountIdSavingTopic);

        var userEmployerId = "emp123";
        var employerAccountId = "acc_emp_123";

        // Act
        await producerService.SaveEmployerAccountIdAsync(userEmployerId, employerAccountId, CancellationToken.None);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var consumeResult = consumer.Consume(cts.Token);

        // Assert
        Assert.NotNull(consumeResult);
        var consumedDto = JsonSerializer.Deserialize<SaveEmployerAccountIdDto>(consumeResult.Message.Value);
        Assert.NotNull(consumedDto);
        Assert.Equal(userEmployerId, consumedDto.UserId);
        Assert.Equal(employerAccountId, consumedDto.EmployerAccountId);
    }

    [Fact]
    public async Task SaveFreelancerAccountIdAsync_ValidData_ShouldProduceMessage()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var producerService = scope.ServiceProvider.GetRequiredService<IAccountsProducerService>();
        var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            GroupId = "test_group_freelancer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(kafkaSettings.FreelancerAccountIdSavingTopic);

        var userFreelancerId = "free123";
        var freelancerAccountId = "acc_free_123";

        // Act
        await producerService.SaveFreelancerAccountIdAsync(userFreelancerId, freelancerAccountId, CancellationToken.None);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var consumeResult = consumer.Consume(cts.Token);

        // Assert
        Assert.NotNull(consumeResult);
        var consumedDto = JsonSerializer.Deserialize<SaveFreelancerAccountIdDto>(consumeResult.Message.Value);
        Assert.NotNull(consumedDto);
        Assert.Equal(userFreelancerId, consumedDto.UserId);
        Assert.Equal(freelancerAccountId, consumedDto.FreelancerAccountId);
    }
}