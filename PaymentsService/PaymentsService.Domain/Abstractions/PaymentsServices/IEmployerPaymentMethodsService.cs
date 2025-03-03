using PaymentsService.Domain.Models;

namespace PaymentsService.Domain.Abstractions.PaymentsServices;

public interface IEmployerPaymentMethodsService
{
    Task SavePaymentMethodAsync(Guid userId, Guid paymentMethodId, CancellationToken cancellationToken);
    Task<IEnumerable<PaymentMethodModel>> GetPaymentMethodsAsync(Guid userId, CancellationToken cancellationToken);
}