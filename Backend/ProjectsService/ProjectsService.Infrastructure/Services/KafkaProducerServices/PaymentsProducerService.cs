using Confluent.Kafka;
using Microsoft.Extensions.Options;
using ProjectsService.Application.Exceptions;
using ProjectsService.Domain.Abstractions.KafkaProducerServices;

namespace ProjectsService.Infrastructure.Services.KafkaProducerServices;

public class PaymentsProducerService : IPaymentsProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _paymentCancellationTopic;
    
    public PaymentsProducerService(IOptions<KafkaSettings> options)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };
        
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        _paymentCancellationTopic = options.Value.PaymentCancellationTopic;
    }

    public async Task CancelPaymentAsync(string paymentId, CancellationToken cancellationToken)
    {
        try
        {
            await _producer.ProduceAsync(_paymentCancellationTopic, new Message<Null, string>
            {
                Value = paymentId
            }, cancellationToken);
        }
        catch (ProduceException<Null, string> ex)
        {
            throw new BadRequestException(
                $"Payment was not successfully cancelled. Producer exception: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException("Payment was not successfully cancelled.");
        }
    }
}