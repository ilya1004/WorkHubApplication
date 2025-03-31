namespace ProjectsService.Domain.Abstractions.KafkaProducerServices;

public interface IPaymentsProducerService
{
    Task CancelPaymentAsync(string paymentId, CancellationToken cancellationToken);
}