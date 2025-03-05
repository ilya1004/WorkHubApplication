using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripePaymentMethodsService(IMapper mapper) : IPaymentMethodsService
{
    private readonly PaymentMethodService _paymentMethodService = new();
    private readonly CustomerPaymentMethodService _customerPaymentMethodService = new();
    
    public async Task SavePaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        await _paymentMethodService.AttachAsync(
            paymentMethodId,
            new PaymentMethodAttachOptions 
            {
                Customer = employerCustomerId, 
            },
            cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<PaymentMethodModel>> GetPaymentMethodsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        if (string.IsNullOrEmpty(employerCustomerId))
        {
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }

        var paymentMethods = await _customerPaymentMethodService.ListAsync(
            employerCustomerId, cancellationToken: cancellationToken);

        return paymentMethods.Data.Select(mapper.Map<PaymentMethodModel>);
    }

    public async Task DeletePaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        if (string.IsNullOrEmpty(employerCustomerId))
        {
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
        
        var paymentMethods = await _customerPaymentMethodService.ListAsync(
            employerCustomerId, cancellationToken: cancellationToken);

        if (paymentMethods.All(pm => pm.Id != paymentMethodId))
        {
            throw new NotFoundException($"You do not have saved Payment method with ID '{userId}'.");
        }
        
        await _paymentMethodService.DetachAsync(paymentMethodId, cancellationToken: cancellationToken);
    } 
}