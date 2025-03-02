using PaymentsService.Domain.DTOs;

namespace PaymentsService.Domain.Abstractions.PaymentsServices;

public interface IEmployerPaymentsService
{
    Task<string> SetupPaymentMethodAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync(Guid userId, CancellationToken cancellationToken);
}