using PaymentsService.Domain.DTOs;

namespace PaymentsService.Domain.Abstractions.PaymentsServices;

public interface IEmployerPaymentsService
{
    Task<string> CreateSetupIntent(Guid userId, CancellationToken cancellationToken);
    Task SavePaymentMethodAsync(Guid userId, Guid paymentMethodId, CancellationToken cancellationToken);
    Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync(Guid userId, CancellationToken cancellationToken);
    Task CreatePaymentIntentWithSavedMethodAsync(Guid userId, Guid projectId, CancellationToken cancellationToken);
}