using PaymentsService.Domain.Models;

namespace PaymentsService.Domain.Abstractions.PaymentsServices;

public interface IPaymentMethodsService
{
    Task SavePaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken cancellationToken);
    Task<IEnumerable<PaymentMethodModel>> GetPaymentMethodsAsync(Guid userId, CancellationToken cancellationToken);
    Task DeletePaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken cancellationToken);
}