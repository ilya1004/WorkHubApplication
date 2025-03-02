namespace PaymentsService.Domain.Abstractions.PaymentsServices;

public interface IEmployerPaymentsService
{
    Task<string> SetupPaymentMethodAsync(Guid userId);
}