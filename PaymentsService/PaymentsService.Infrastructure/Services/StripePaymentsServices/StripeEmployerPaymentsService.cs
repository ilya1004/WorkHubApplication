using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripeEmployerPaymentsService : IEmployerPaymentsService
{
    private readonly SetupIntentService _intentService = new();
    public async Task<string> SetupPaymentMethodAsync(Guid userId)
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
        
        var setupIntent = await _intentService.CreateAsync(options);

        return setupIntent.ClientSecret;
    }
}