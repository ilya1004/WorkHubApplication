using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripeEmployerPaymentsService(
    IMapper mapper) : IEmployerPaymentsService
{
    private readonly SetupIntentService _intentService = new();
    private readonly CustomerPaymentMethodService _customerPaymentMethodService = new();
    public async Task<string> SetupPaymentMethodAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employerStripeCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        if (string.IsNullOrEmpty(employerStripeCustomerId))
        {
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
        
        var options = new SetupIntentCreateOptions
        {
            Customer = employerStripeCustomerId,
            PaymentMethodTypes = ["card"]
        };
        
        var setupIntent = await _intentService.CreateAsync(options, cancellationToken: cancellationToken);

        return setupIntent.ClientSecret;
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employerStripeCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC
        
        if (string.IsNullOrEmpty(employerStripeCustomerId))
        {
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }

        var options = new CustomerPaymentMethodListOptions();
        
        var paymentMethods = await _customerPaymentMethodService.ListAsync(
            employerStripeCustomerId, options, cancellationToken: cancellationToken);

        return paymentMethods.Data.Select(mapper.Map<PaymentMethodDto>);
    }
}