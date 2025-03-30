using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;

namespace PaymentsService.Infrastructure.Services.KafkaProducerServices;

public class PaymentsProducerService : IPaymentsProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _paymentIntentSavingTopic;
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

        _paymentIntentSavingTopic = options.Value.PaymentIntentSavingTopic;
        
        _logger.LogInformation("Using topic: {Topic}", _paymentIntentSavingTopic);
    }

    public async Task SavePaymentIntentIdAsync(string projectId, string paymentIntentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving payment intent ID {PaymentIntentId} for project {ProjectId}", 
            paymentIntentId, projectId);
            
        try
        {
            var dto = new SavePaymentIntentIdDto
            {
                ProjectId = projectId,
                PaymentIntentId = paymentIntentId
            };

            var jsonData = JsonSerializer.Serialize(dto);
                
            var result = await _producer.ProduceAsync(_paymentIntentSavingTopic, new Message<Null, string>
            {
                Value = jsonData
            }, cancellationToken);

            _logger.LogInformation("Successfully saved payment intent ID {PaymentIntentId} for project {ProjectId}. " +
                                   "Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                paymentIntentId, projectId, result.Topic, result.Partition, result.Offset);
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, "Failed to save payment intent ID {PaymentIntentId}. Kafka error: {Error}", 
                paymentIntentId, ex.Error.Reason);
            
            throw new BadRequestException($"Payment intent ID was not successfully saved. Producer exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save payment intent ID {PaymentIntentId}", paymentIntentId);
            
            throw new BadRequestException("Payment intent ID was not successfully saved.");
        }
    }
}