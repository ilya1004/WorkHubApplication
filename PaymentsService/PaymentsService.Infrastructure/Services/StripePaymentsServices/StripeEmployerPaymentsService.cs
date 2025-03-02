using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripeEmployerPaymentsService(IMapper mapper) : IEmployerPaymentsService
{
    private readonly SetupIntentService _intentService = new();
    private readonly CustomerPaymentMethodService _customerPaymentMethodService = new();
    private readonly PaymentMethodService _paymentMethodService = new();
    private readonly PaymentIntentService _paymentIntentService = new();
    public async Task<string> CreateSetupIntent(Guid userId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        if (string.IsNullOrEmpty(employerCustomerId))
        {
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
        
        var options = new SetupIntentCreateOptions
        {
            Customer = employerCustomerId,
            PaymentMethodTypes = ["card"]
        };
        
        var setupIntent = await _intentService.CreateAsync(options, cancellationToken: cancellationToken);

        return setupIntent.ClientSecret;
    }
    
    public async Task SavePaymentMethodAsync(Guid userId, Guid paymentMethodId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        await _paymentMethodService.AttachAsync(paymentMethodId.ToString(), new PaymentMethodAttachOptions 
            {
                Customer = employerCustomerId, 
            },
            cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC
        
        if (string.IsNullOrEmpty(employerCustomerId))
        {
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }

        var options = new CustomerPaymentMethodListOptions();
        
        var paymentMethods = await _customerPaymentMethodService.ListAsync(
            employerCustomerId, options, cancellationToken: cancellationToken);

        return paymentMethods.Data.Select(mapper.Map<PaymentMethodDto>);
    }

    public async Task CreatePaymentIntentWithSavedMethodAsync(Guid userId, Guid projectId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC
        
        if (string.IsNullOrEmpty(employerCustomerId))
        {
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }

        var paymentMethods = await _customerPaymentMethodService.ListAsync(
            employerCustomerId, cancellationToken: cancellationToken);

        if (!paymentMethods.Any())
        {
            throw new BadRequestException("You must save at least one payment method.");
        }

        var project = new ProjectDto(); // This data will be requested from Projects Service via gRPC
        
        var options = new PaymentIntentCreateOptions
        {
            Amount = project.Budget * 100,
            Currency = "usd",
            Customer = employerCustomerId,
            PaymentMethod = paymentMethods.First().Id,
            Confirm = true,
            CaptureMethod = "manual",
            TransferGroup = projectId.ToString(),
        };
        
        await _paymentIntentService.CreateAsync(options, cancellationToken: cancellationToken);
    }
}