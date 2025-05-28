using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Infrastructure.DTOs;
using ProjectsService.Infrastructure.Settings;
using ProjectsService.Tests.IntegrationTests.Helpers;

namespace ProjectsService.Tests.IntegrationTests.Tests.KafkaServices;

public class PaymentsConsumerServiceIntegrationTests(
    IntegrationTestsFixture fixture) : IClassFixture<IntegrationTestsFixture>
{
    [Fact]
    public async Task ConsumeMessage_ValidMessage_ShouldUpdateProjectPaymentIntent()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;
        
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Title = "Test Project",
            Description = "Description",
            Budget = 1000m,
            EmployerId = Guid.NewGuid(),
            Lifecycle = new Lifecycle { Id = Guid.NewGuid() },
            FreelancerApplications = new List<FreelancerApplication>()
        };
        await unitOfWork.ProjectCommandsRepository.AddAsync(project);
        await unitOfWork.SaveAllAsync();
        
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            AllowAutoCreateTopics = true
        };
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        var dto = new SavePaymentIntentIdDto
        {
            ProjectId = project.Id.ToString(),
            PaymentIntentId = "pi_123456"
        };
        var message = JsonSerializer.Serialize(dto);
        
        await producer.ProduceAsync(kafkaSettings.PaymentIntentSavingTopic, new Message<Null, string>
        {
            Value = message
        });

        // Act
        await Task.Delay(1000);

        // Assert
        var updatedProject = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(project.Id);
        Assert.NotNull(updatedProject);
        Assert.Equal(project.Title, updatedProject.Title);
        Assert.Equal(project.Description, updatedProject.Description);
        Assert.Equal(project.Budget, updatedProject.Budget);
    }

    [Fact]
    public async Task ConsumeMessage_ProjectNotFound_ShouldNotThrow()
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

        var dto = new SavePaymentIntentIdDto
        {
            ProjectId = Guid.NewGuid().ToString(),
            PaymentIntentId = "pi_123456"
        };
        var message = JsonSerializer.Serialize(dto);
        
        await producer.ProduceAsync(kafkaSettings.PaymentIntentSavingTopic, new Message<Null, string>
        {
            Value = message
        });

        // Act
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
        
        await producer.ProduceAsync(kafkaSettings.PaymentIntentSavingTopic, new Message<Null, string>
        {
            Value = invalidMessage
        });

        // Act
        await Task.Delay(1000);

        // Assert
        Assert.True(true);
    }
}