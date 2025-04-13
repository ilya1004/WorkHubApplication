using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripePaymentMethodsService(
    IMapper mapper,
    IEmployersGrpcClient employersGrpcClient,
    ILogger<StripePaymentMethodsService> logger) : IPaymentMethodsService
{
    private readonly CustomerPaymentMethodService _customerPaymentMethodService = new();
    private readonly PaymentMethodService _paymentMethodService = new();

    public async Task SavePaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Saving payment method {PaymentMethodId} for user {UserId}", paymentMethodId, userId);

        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);
        
        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
        {
            logger.LogWarning("Employer account not found for user {UserId}", userId);
            
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
        
        try
        {
            logger.LogInformation("Retrieving payment method {PaymentMethodId}", paymentMethodId);
            
            var paymentMethod = await _paymentMethodService.GetAsync(paymentMethodId, cancellationToken: cancellationToken);

            if (paymentMethod is null)
            {
                logger.LogError("Payment method {PaymentMethodId} not found", paymentMethodId);
                
                throw new NotFoundException($"Payment method with ID '{paymentMethodId}' not found.");
            }
            
            logger.LogInformation("Attaching payment method {PaymentMethodId} to customer {CustomerId}", 
                paymentMethodId, employer.EmployerCustomerId);
            
            await _paymentMethodService.AttachAsync(
                paymentMethodId,
                new PaymentMethodAttachOptions
                {
                    Customer = employer.EmployerCustomerId
                },
                cancellationToken: cancellationToken);
            
            logger.LogInformation("Payment method {PaymentMethodId} saved successfully for user {UserId}", 
                paymentMethodId, userId);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error saving payment method: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving payment method for user {UserId}", userId);
            
            throw new BadRequestException($"Could not save your Payment method for employer with ID '{userId}'.");
        }
    }

    public async Task<IEnumerable<PaymentMethodModel>> GetPaymentMethodsAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting payment methods for user {UserId}", userId);

        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);
        
        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
        {
            logger.LogWarning("Employer account not found for user {UserId}", userId);
            
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
        
        try
        {
            logger.LogInformation("Listing payment methods for customer {CustomerId}", employer.EmployerCustomerId);
            
            var paymentMethods = await _customerPaymentMethodService.ListAsync(
                employer.EmployerCustomerId, cancellationToken: cancellationToken);
            
            logger.LogInformation("Retrieved {Count} payment methods for user {UserId}", 
                paymentMethods.Data.Count, userId);
            
            return paymentMethods.Data.Select(mapper.Map<PaymentMethodModel>);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error getting payment methods: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting payment methods for user {UserId}", userId);
            
            throw new BadRequestException($"Could not get your Payment methods for employer with ID '{userId}'.");
        }
    }

    public async Task DeletePaymentMethodAsync(Guid userId, string paymentMethodId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting payment method {PaymentMethodId} for user {UserId}", paymentMethodId, userId);

        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);
        
        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
        {
            logger.LogWarning("Employer account not found for user {UserId}", userId);
            
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
        
        try
        {
            logger.LogInformation("Listing payment methods for customer {CustomerId}", employer.EmployerCustomerId);
            
            var paymentMethods = await _customerPaymentMethodService.ListAsync(
                employer.EmployerCustomerId, cancellationToken: cancellationToken);

            var paymentMethod = paymentMethods.FirstOrDefault(pm => pm.Id == paymentMethodId);

            if (paymentMethod is null)
            {
                logger.LogError("Payment method {PaymentMethodId} not found for user {UserId}", paymentMethodId, userId);
                
                throw new NotFoundException($"You do not have a saved Payment method with ID '{paymentMethodId}'.");
            }
            
            logger.LogInformation("Detaching payment method {PaymentMethodId}", paymentMethodId);
            
            await _paymentMethodService.DetachAsync(paymentMethodId, cancellationToken: cancellationToken);
            
            logger.LogInformation("Payment method {PaymentMethodId} deleted successfully for user {UserId}", 
                paymentMethodId, userId);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error deleting payment method: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting payment method for user {UserId}", userId);
            
            throw new BadRequestException($"Could not delete Payment method with ID '{paymentMethodId}'.");
        }
    }
}
