using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Infrastructure.Settings;

namespace PaymentsService.Infrastructure.Services.KafkaProducerServices;

public class PaymentsProducerService : IPaymentsProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _paymentsTopic;

    public PaymentsProducerService(IOptions<KafkaSettings> options)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All,
            RetryBackoffMs = 500,   // Задержка перед ретраем
            MessageTimeoutMs = 30000,  // 30 секунд ожидания
            SocketTimeoutMs = 30000,   // 30 секунд ожидания соединения
        };
        
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        _paymentsTopic = options.Value.PaymentsTopic;
    }

    public async Task SavePaymentIntent(string paymentIntentId, CancellationToken cancellationToken)
    {
        try
        {
            await _producer.ProduceAsync(_paymentsTopic, new Message<Null, string>
            {
                Value = paymentIntentId,
                Headers = [new Header("event_type", Encoding.UTF8.GetBytes($"{nameof(SavePaymentIntent)}"))]
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