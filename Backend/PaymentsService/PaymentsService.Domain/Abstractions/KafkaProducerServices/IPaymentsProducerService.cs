namespace PaymentsService.Domain.Abstractions.KafkaProducerServices;

public interface IPaymentsProducerService
{
    Task SavePaymentIntentIdAsync(string projectId, string paymentIntentId, CancellationToken cancellationToken);
}