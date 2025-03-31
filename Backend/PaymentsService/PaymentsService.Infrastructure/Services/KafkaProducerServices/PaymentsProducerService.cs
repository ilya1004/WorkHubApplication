using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Infrastructure.DTOs;

namespace PaymentsService.Infrastructure.Services.KafkaProducerServices;

public class PaymentsProducerService : IPaymentsProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _paymentIntentSavingTopic;

    public PaymentsProducerService(IOptions<KafkaSettings> options)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };
        
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        _paymentIntentSavingTopic = options.Value.PaymentIntentSavingTopic;
    }

    public async Task SavePaymentIntentIdAsync(string projectId, string paymentIntentId, 
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = new SavePaymentIntentIdDto
            {
                ProjectId = projectId,
                PaymentIntentId = paymentIntentId
            };

            var jsonData = JsonSerializer.Serialize(dto);
                
            await _producer.ProduceAsync(_paymentIntentSavingTopic, new Message<Null, string>
            {
                Value = jsonData
            }, cancellationToken);
        }
        catch (ProduceException<Null, string> ex)
        {
            throw new BadRequestException(
                $"Payment intent ID was not successfully saved. Producer exception: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException("Payment intent ID was not successfully saved.");
        }
    }
}