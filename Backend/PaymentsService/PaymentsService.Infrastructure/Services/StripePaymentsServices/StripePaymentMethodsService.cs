using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripePaymentMethodsService(
    IMapper mapper,
    IEmployersGrpcClient employersGrpcClient) : IPaymentMethodsService
{
    private readonly CustomerPaymentMethodService _customerPaymentMethodService = new();
    private readonly PaymentMethodService _paymentMethodService = new();

    public async Task SavePaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken cancellationToken)
    {
        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);
        
        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        
        try
        {
            await _paymentMethodService.AttachAsync(
                paymentMethodId,
                new PaymentMethodAttachOptions
                {
                    Customer = employer.EmployerCustomerId
                },
                cancellationToken: cancellationToken);
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not save your Payment method for employer with ID '{userId}'.");
        }
    }

    public async Task<IEnumerable<PaymentMethodModel>> GetPaymentMethodsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);
        
        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        
        try
        {
            var paymentMethods = await _customerPaymentMethodService.ListAsync(
                employer.EmployerCustomerId, cancellationToken: cancellationToken);

            return paymentMethods.Data.Select(mapper.Map<PaymentMethodModel>);
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not get your Payment methods for employer with ID '{userId}'.");
        }
    }

    public async Task DeletePaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken cancellationToken)
    {
        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);
        
        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");

        var paymentMethods = await _customerPaymentMethodService.ListAsync(
            employer.EmployerCustomerId, cancellationToken: cancellationToken);

        if (paymentMethods.All(pm => pm.Id != paymentMethodId))
            throw new NotFoundException($"You do not have saved Payment method with ID '{userId}'.");

        try
        {
            await _paymentMethodService.DetachAsync(paymentMethodId, cancellationToken: cancellationToken);
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not delete Payment method with ID '{paymentMethodId}'.");
        }
    }
}