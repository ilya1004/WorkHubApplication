using Confluent.Kafka;
using Microsoft.Extensions.Options;
using ProjectsService.Application.Exceptions;
using ProjectsService.Domain.Abstractions.KafkaProducerServices;

namespace ProjectsService.Infrastructure.Services.KafkaProducerServices;

public class PaymentsProducerService : IPaymentsProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _paymentCancellationTopic;
    private readonly ILogger<PaymentsProducerService> _logger;
    
    public PaymentsProducerService(
        IOptions<KafkaSettings> options,
        ILogger<PaymentsProducerService> logger)
    {
        _logger = logger;
        
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };
        
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        
        _logger.LogInformation("Kafka producer initialized");

        _paymentCancellationTopic = options.Value.PaymentCancellationTopic;
        
        _logger.LogInformation("Using payment cancellation topic: {Topic}", _paymentCancellationTopic);
    }

    public async Task CancelPaymentAsync(string paymentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling payment {PaymentId}", paymentId);
        
        try
        {
            var result = await _producer.ProduceAsync(_paymentCancellationTopic, new Message<Null, string>
            {
                Value = paymentId
            }, cancellationToken);

            _logger.LogInformation("Successfully sent payment cancellation for {PaymentId}. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                paymentId, result.Topic, result.Partition, result.Offset.Value);
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, "Failed to cancel payment {PaymentId}. Kafka error: {Error}", 
                paymentId, ex.Error.Reason);
            
            throw new BadRequestException($"Payment was not successfully cancelled. Producer exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel payment {PaymentId}", paymentId);
            
            throw new BadRequestException("Payment was not successfully cancelled.");
        }
    }
}