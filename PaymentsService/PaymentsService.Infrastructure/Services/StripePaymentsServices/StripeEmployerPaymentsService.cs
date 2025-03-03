using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripeEmployerPaymentsService(
    IMapper mapper,
    ITransfersService transfersService) : IEmployerPaymentsService
{
    private readonly SetupIntentService _intentService = new();
    private readonly CustomerPaymentMethodService _customerPaymentMethodService = new();
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
            Metadata = new Dictionary<string, string>
            {
                { "project_id", projectId.ToString() },
                { "freelancer_id", project.FreelancerId.ToString() }
            }
        };
        
        var paymentIntent = await _paymentIntentService.CreateAsync(options, cancellationToken: cancellationToken);
        // This data will be sent to Projects Service via gRPC
    }

    public async Task ConfirmPaymentForProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken)
    {
        var project = new ProjectDto(); // This data will be requested from Projects Service via gRPC

        if (project.PaymentIntentId is null)
        {
            throw new NotFoundException("This project does not have an attached payment intent.");
        }

        var freelancer = new FreelancerDto(); // This data will be requested from Identity Service via gRPC
        
        if (freelancer is null)
        {
            throw new NotFoundException($"Freelancer to project with ID '{projectId}' not found.");
        }

        if (freelancer.StripeAccountId is null)
        {
            throw new NotFoundException($"Freelancer's Stripe Account to project with ID '{projectId}' not found.");
        }
        
        var paymentIntent = await _paymentIntentService.GetAsync(project.PaymentIntentId, cancellationToken: cancellationToken);

        if (paymentIntent == null)
        {
            throw new NotFoundException($"Payment Intent with ID '{project.PaymentIntentId}' not found for this project.");
        }

        var confirmOptions = new PaymentIntentCaptureOptions
        {
            AmountToCapture = paymentIntent.Amount
        };

        var capturedPaymentIntent = await _paymentIntentService.CaptureAsync(
            paymentIntent.Id, confirmOptions, cancellationToken: cancellationToken);
        
        await transfersService.TransferFundsToFreelancer(
            mapper.Map<PaymentIntentModel>(capturedPaymentIntent), freelancer.StripeAccountId, cancellationToken);
    }
    
}