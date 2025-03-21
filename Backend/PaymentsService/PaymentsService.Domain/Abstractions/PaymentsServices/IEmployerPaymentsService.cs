namespace PaymentsService.Domain.Abstractions.PaymentsServices;

public interface IEmployerPaymentsService
{
    Task<string> CreateSetupIntent(Guid userId, CancellationToken cancellationToken);
    Task CreatePaymentIntentWithSavedMethodAsync(Guid userId, Guid projectId, string paymentMethodId, CancellationToken cancellationToken);
    Task ConfirmPaymentForProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken);
    Task CancelPaymentIntentForProjectAsync(string paymentIntentId, CancellationToken cancellationToken);
}