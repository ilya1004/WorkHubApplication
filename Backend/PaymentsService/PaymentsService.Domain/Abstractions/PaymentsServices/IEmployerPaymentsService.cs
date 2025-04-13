namespace PaymentsService.Domain.Abstractions.PaymentsServices;

public interface IEmployerPaymentsService
{
    Task CreatePaymentIntentWithSavedMethodAsync(Guid userId, Guid projectId, string paymentMethodId, CancellationToken cancellationToken);
    Task ConfirmPaymentForProjectAsync(Guid projectId, CancellationToken cancellationToken);
    Task CancelPaymentIntentForProjectAsync(string paymentIntentId, CancellationToken cancellationToken);
}