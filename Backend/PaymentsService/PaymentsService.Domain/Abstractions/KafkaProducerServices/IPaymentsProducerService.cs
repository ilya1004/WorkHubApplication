namespace PaymentsService.Domain.Abstractions.KafkaProducerServices;

public interface IPaymentsProducerService
{
    Task SavePaymentIntent(string paymentIntentId, CancellationToken cancellationToken);
}