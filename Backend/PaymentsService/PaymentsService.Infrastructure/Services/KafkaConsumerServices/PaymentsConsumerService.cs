using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Infrastructure.Services.KafkaConsumerServices;

public class PaymentsConsumerService(
    IOptions<KafkaSettings> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<PaymentsConsumerService> logger) : BackgroundService
{
    private IConsumer<Ignore, string> _consumer = null!;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting payments consumer service");
        
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = "payments_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(options.Value.PaymentCancellationTopic);
        
        logger.LogInformation("Subscribed to topic: {Topic}", options.Value.PaymentCancellationTopic);

        await Task.Run(() => ConsumeMessagesAsync(stoppingToken), stoppingToken);
    }

    private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting to consume messages");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                 
                    logger.LogInformation("Received message from Kafka. ProjectId: {ProjectId}", result.Message.Value);
                    
                    using var scope = serviceScopeFactory.CreateScope();
                    var employerPaymentsService = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

                    logger.LogInformation("Processing payment cancellation for project: {ProjectId}", result.Message.Value);
                    
                    await employerPaymentsService.CancelPaymentIntentForProjectAsync(result.Message.Value, stoppingToken);
                    
                    logger.LogInformation("Successfully processed payment cancellation for project: {ProjectId}", result.Message.Value);
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Error consuming Kafka message. Error: {ErrorMessage}", ex.Error.Reason);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing payment cancellation. Error: {ErrorMessage}", ex.Message);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Payments consumer service is stopping due to cancellation request");
        }
        finally
        {
            _consumer.Close();
            
            logger.LogInformation("Kafka consumer closed successfully");
        }
    }
}