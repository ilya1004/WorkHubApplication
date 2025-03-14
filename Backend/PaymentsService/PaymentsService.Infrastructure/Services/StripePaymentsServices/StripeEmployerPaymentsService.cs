using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripeEmployerPaymentsService(
    IMapper mapper,
    ITransfersService transfersService) : IEmployerPaymentsService
{
    private readonly CustomerPaymentMethodService _customerPaymentMethodService = new();
    private readonly SetupIntentService _intentService = new();
    private readonly PaymentIntentService _paymentIntentService = new();

    public async Task<string> CreateSetupIntent(Guid userId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        if (employerCustomerId is null) throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");

        var options = new SetupIntentCreateOptions
        {
            Customer = employerCustomerId,
            PaymentMethodTypes = ["card"]
        };

        var setupIntent = await _intentService.CreateAsync(options, cancellationToken: cancellationToken);

        return setupIntent.ClientSecret;
    }

    public async Task CreatePaymentIntentWithSavedMethodAsync(Guid userId, Guid projectId, string paymentMethodId,
        CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        if (employerCustomerId is null) throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");

        var paymentMethod = await _customerPaymentMethodService.GetAsync(
            employerCustomerId, paymentMethodId, cancellationToken: cancellationToken);

        if (paymentMethod is null) throw new BadRequestException($"Payment method with ID '{paymentMethodId}' not found.");

        var project = new ProjectDto(); // This data will be requested from Projects Service via gRPC

        var options = new PaymentIntentCreateOptions
        {
            Amount = project.Budget * 100,
            Currency = "usd",
            Customer = employerCustomerId,
            PaymentMethod = paymentMethod.Id,
            Confirm = true,
            CaptureMethod = "manual",
            TransferGroup = projectId.ToString(),
            Metadata = new Dictionary<string, string>
            {
                { "project_id", projectId.ToString() },
                { "freelancer_id", project.FreelancerId.ToString() }
            }
        };

        try
        {
            var paymentIntent = await _paymentIntentService.CreateAsync(options, cancellationToken: cancellationToken);
            // This data will be sent to Projects Service via gRPC
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not create Payment intent for project with ID '{projectId}'.");
        }
    }

    public async Task ConfirmPaymentForProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken)
    {
        var project = new ProjectDto(); // This data will be requested from Projects Service via gRPC

        if (project.PaymentIntentId is null) throw new NotFoundException("This project does not have an attached Payment Intent.");

        var freelancer = new FreelancerDto(); // This data will be requested from Identity Service via gRPC

        if (freelancer.StripeAccountId is null)
            throw new NotFoundException($"Freelancer's Stripe Account to project with ID '{projectId}' not found.");

        var paymentIntent = await _paymentIntentService.GetAsync(project.PaymentIntentId, cancellationToken: cancellationToken);

        if (paymentIntent is null)
            throw new NotFoundException($"Payment Intent with ID '{project.PaymentIntentId}' not found for this project.");

        var confirmOptions = new PaymentIntentCaptureOptions
        {
            AmountToCapture = paymentIntent.Amount
        };

        try
        {
            var capturedPaymentIntent = await _paymentIntentService.CaptureAsync(
                paymentIntent.Id, confirmOptions, cancellationToken: cancellationToken);

            await transfersService.TransferFundsToFreelancer(
                mapper.Map<PaymentIntentModel>(capturedPaymentIntent),
                project.Id,
                freelancer.StripeAccountId,
                cancellationToken);
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not confirm Payment for project with ID '{projectId}'.");
        }
    }

    // This method will be called from Projects Service only via gRPC
    public async Task CancelPaymentForProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken)
    {
        var project = new ProjectDto(); // This data will be requested from Projects Service via gRPC

        if (project.PaymentIntentId is null) throw new NotFoundException("This project does not have an attached Payment Intent.");

        var paymentIntent = await _paymentIntentService.GetAsync(project.PaymentIntentId, cancellationToken: cancellationToken);

        if (paymentIntent is null) throw new NotFoundException($"Payment Intent with ID '{project.PaymentIntentId}' not found.");

        try
        {
            await _paymentIntentService.CancelAsync(project.PaymentIntentId, cancellationToken: cancellationToken);
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not cancel Payment for project with ID '{projectId}'.");
        }
    }
}