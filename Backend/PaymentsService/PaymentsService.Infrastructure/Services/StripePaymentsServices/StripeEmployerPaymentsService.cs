using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripeEmployerPaymentsService(
    IMapper mapper,
    ITransfersService transfersService,
    IEmployersGrpcClient employersGrpcClient,
    IProjectsGrpcClient projectsGrpcClient,
    IFreelancersGrpcClient freelancersGrpcClient) : IEmployerPaymentsService
{
    private readonly CustomerPaymentMethodService _customerPaymentMethodService = new();
    private readonly SetupIntentService _intentService = new();
    private readonly PaymentIntentService _paymentIntentService = new();

    public async Task<string> CreateSetupIntent(Guid userId, CancellationToken cancellationToken)
    {
        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken); 

        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");

        var options = new SetupIntentCreateOptions
        {
            Customer = employer.EmployerCustomerId,
            PaymentMethodTypes = ["card"]
        };

        var setupIntent = await _intentService.CreateAsync(options, cancellationToken: cancellationToken);

        return setupIntent.ClientSecret;
    }

    public async Task CreatePaymentIntentWithSavedMethodAsync(Guid userId, Guid projectId, string paymentMethodId,
        CancellationToken cancellationToken)
    {
        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");

        var paymentMethod = await _customerPaymentMethodService.GetAsync(
            employer.EmployerCustomerId, paymentMethodId, cancellationToken: cancellationToken);

        if (paymentMethod is null) throw new BadRequestException($"Payment method with ID '{paymentMethodId}' not found.");
        
        var project = await projectsGrpcClient.GetProjectByIdAsync(projectId.ToString(), cancellationToken);

        var options = new PaymentIntentCreateOptions
        {
            Amount = project.Budget * 100,
            Currency = "usd",
            Customer = employer.EmployerCustomerId,
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
            
            // This data will be sent to Projects Service via Kafka
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
        var project = await projectsGrpcClient.GetProjectByIdAsync(projectId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(project.PaymentIntentId)) throw new NotFoundException("This project does not have an attached Payment Intent.");

        var freelancer = await freelancersGrpcClient.GetFreelancerByIdAsync(projectId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(freelancer.StripeAccountId))
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

    // This method will be called from Projects Service only via Kafka
    public async Task CancelPaymentForProjectAsync(string paymentIntentId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(paymentIntentId)) throw new NotFoundException("This project does not have an attached Payment Intent.");

        var paymentIntent = await _paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

        if (paymentIntent is null) throw new NotFoundException($"Payment Intent with ID '{paymentIntentId}' not found.");

        try
        {
            await _paymentIntentService.CancelAsync(paymentIntentId, cancellationToken: cancellationToken);
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not cancel Payment with ID '{paymentIntent}'.");
        }
    }
}