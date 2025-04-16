using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Infrastructure.Services.KafkaConsumerServices;
using PaymentsService.Infrastructure.Settings;
using PaymentsService.Tests.IntegrationTests.Helpers;

namespace PaymentsService.Tests.IntegrationTests.Tests.KafkaServices;

public class PaymentsConsumerServiceIntegrationTests(
    KafkaIntegrationTestsFixture fixture) : IClassFixture<KafkaIntegrationTestsFixture>
{
    [Fact]
    public async Task ConsumeMessage_ValidCancellationMessage_ShouldCallCancelPaymentIntent()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;
        var employerPaymentsServiceMock = new Mock<IEmployerPaymentsService>();
        var projectId = Guid.NewGuid().ToString();
        
        employerPaymentsServiceMock
            .Setup(s => s.CancelPaymentIntentForProjectAsync(projectId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEmployerPaymentsService)))
            .Returns(employerPaymentsServiceMock.Object);
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(serviceScopeMock.Object);
        
        var consumerService = new PaymentsConsumerService(
            Options.Create(kafkaSettings),
            serviceScopeFactoryMock.Object,
            fixture.Factory.Services.GetRequiredService<ILogger<PaymentsConsumerService>>()
        );
        
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var consumeTask = consumerService.StartAsync(cts.Token);
        
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            AllowAutoCreateTopics = true
        };
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        await producer.ProduceAsync(kafkaSettings.PaymentCancellationTopic, new Message<Null, string>
        {
            Value = projectId
        }, cts.Token);

        // Act
        await Task.Delay(2000, cts.Token);
        
        await cts.CancelAsync();
        await consumeTask;

        // Assert
        employerPaymentsServiceMock.Verify(
            s => s.CancelPaymentIntentForProjectAsync(projectId, It.IsAny<CancellationToken>()),
            Times.Once()
        );
    }

    [Fact]
    public async Task ConsumeMessage_InvalidMessage_ShouldNotThrow()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;
        var employerPaymentsServiceMock = new Mock<IEmployerPaymentsService>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEmployerPaymentsService)))
            .Returns(employerPaymentsServiceMock.Object);
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(serviceScopeMock.Object);

        var consumerService = new PaymentsConsumerService(
            Options.Create(kafkaSettings),
            serviceScopeFactoryMock.Object,
            fixture.Factory.Services.GetRequiredService<ILogger<PaymentsConsumerService>>()
        );

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var consumeTask = consumerService.StartAsync(cts.Token);
        
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            AllowAutoCreateTopics = true
        };
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        await producer.ProduceAsync(kafkaSettings.PaymentCancellationTopic, new Message<Null, string>
        {
            Value = "invalid_message"
        }, cts.Token);

        // Act
        await Task.Delay(3000, cts.Token);
        
        await cts.CancelAsync();
        await consumeTask;

        // Assert
        employerPaymentsServiceMock.Verify(
            s => s.CancelPaymentIntentForProjectAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
    }
}