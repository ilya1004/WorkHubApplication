using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.Services.StripePaymentsServices;

public class StripeEmployerPaymentsService(
    IMapper mapper,
    ITransfersService transfersService,
    IPaymentsProducerService paymentsProducerService,
    IEmployersGrpcClient employersGrpcClient,
    IProjectsGrpcClient projectsGrpcClient,
    IFreelancersGrpcClient freelancersGrpcClient,
    ILogger<StripeEmployerPaymentsService> logger) : IEmployerPaymentsService
{
    private readonly CustomerPaymentMethodService _customerPaymentMethodService = new();
    private readonly PaymentIntentService _paymentIntentService = new();
    private readonly AccountService _accountService = new();

    public async Task CreatePaymentIntentWithSavedMethodAsync(Guid userId, Guid projectId, string paymentMethodId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating payment intent for project {ProjectId} with method {PaymentMethodId} by user {UserId}", 
            projectId, paymentMethodId, userId);

        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
        {
            logger.LogWarning("Employer account not found for user {UserId}", userId);
            
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
        
        var project = await projectsGrpcClient.GetProjectByIdAsync(projectId.ToString(), cancellationToken);
        
        try
        {
            logger.LogInformation("Retrieving payment method {PaymentMethodId}", paymentMethodId);
            
            var paymentMethod = await _customerPaymentMethodService.GetAsync(
                employer.EmployerCustomerId, paymentMethodId, cancellationToken: cancellationToken);

            if (paymentMethod is null)
            {
                logger.LogError("Payment method {PaymentMethodId} not found", paymentMethodId);
                
                throw new BadRequestException($"Payment method with ID '{paymentMethodId}' not found.");
            }
            
            var options = new PaymentIntentCreateOptions
            {
                Amount = project.BudgetInCents,
                Currency = "eur",
                Customer = employer.EmployerCustomerId,
                PaymentMethod = paymentMethod.Id,
                Confirm = true,
                CaptureMethod = "manual",
                TransferGroup = projectId.ToString(),
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never"
                },
                Metadata = new Dictionary<string, string>
                {
                    { "project_id", projectId.ToString() },
                }
            };
            
            logger.LogInformation("Creating payment intent with options: {@Options}", options);
            
            var paymentIntent = await _paymentIntentService.CreateAsync(options, cancellationToken: cancellationToken);

            logger.LogInformation("Payment intent {PaymentIntentId} created successfully for project {ProjectId}", 
                paymentIntent.Id, projectId);
            
            await paymentsProducerService.SavePaymentIntentIdAsync(project.Id.ToString(), paymentIntent.Id, cancellationToken);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error creating payment intent: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating payment intent for project {ProjectId}", projectId);
            
            throw new BadRequestException($"Could not create Payment intent for project with ID '{projectId}'.");
        }
    }

    public async Task ConfirmPaymentForProjectAsync(Guid projectId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Confirming payment for project {ProjectId}", projectId);

        var project = await projectsGrpcClient.GetProjectByIdAsync(projectId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(project.PaymentIntentId)) 
        {
            logger.LogWarning("Payment intent not found for project {ProjectId}", projectId);
            
            throw new NotFoundException("This project does not have an attached Payment Intent.");
        }
        
        if (project.FreelancerId is null) 
        {
            logger.LogWarning("Freelancer user not found for project {ProjectId}", projectId);
            
            throw new NotFoundException("This project does not have freelancer.");
        }

        var freelancer = await freelancersGrpcClient.GetFreelancerByIdAsync(project.FreelancerId.ToString()!, cancellationToken);

        if (string.IsNullOrEmpty(freelancer.StripeAccountId))
        {
            logger.LogWarning("Freelancer account not found for project {ProjectId}", projectId);
            
            throw new NotFoundException($"Freelancer's Stripe Account to project with ID '{projectId}' not found.");
        }
        
        var account = await _accountService.GetAsync(freelancer.StripeAccountId, cancellationToken: cancellationToken);
        
        if (!account.ChargesEnabled || !account.PayoutsEnabled)
        {
            logger.LogWarning("Freelancer account {AccountId} is not fully activated.", freelancer.StripeAccountId);
            
            throw new BadRequestException($"Freelancer account '{freelancer.StripeAccountId}' is not fully activated for transfers.");
        }

        try
        {
            logger.LogInformation("Retrieving payment intent {PaymentIntentId}", project.PaymentIntentId);
            
            var paymentIntent = await _paymentIntentService.GetAsync(project.PaymentIntentId, cancellationToken: cancellationToken);

            if (paymentIntent is null)
            {
                logger.LogError("Payment intent {PaymentIntentId} not found", project.PaymentIntentId);
                
                throw new NotFoundException($"Payment Intent with ID '{project.PaymentIntentId}' not found for this project.");
            }
        
            if (paymentIntent.Status != "requires_capture")
            {
                logger.LogWarning("Payment intent {PaymentIntentId} not in capturable state: {Status}", 
                    project.PaymentIntentId, paymentIntent.Status);
                
                throw new BadRequestException($"Payment Intent with ID '{project.PaymentIntentId}' is not in a capturable state.");
            }

            var confirmOptions = new PaymentIntentCaptureOptions
            {
                AmountToCapture = paymentIntent.Amount
            };
            
            logger.LogInformation("Capturing payment intent {PaymentIntentId}", paymentIntent.Id);
            
            var capturedPaymentIntent = await _paymentIntentService.CaptureAsync(
                paymentIntent.Id, confirmOptions, cancellationToken: cancellationToken);

            logger.LogInformation("Payment intent {PaymentIntentId} captured successfully", paymentIntent.Id);

            await transfersService.TransferFundsToFreelancer(
                mapper.Map<PaymentIntentModel>(capturedPaymentIntent),
                project.Id,
                freelancer.StripeAccountId,
                cancellationToken);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error confirming payment: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error confirming payment for project {ProjectId}", projectId);
            
            throw new BadRequestException($"Could not confirm Payment for project with ID '{projectId}'.");
        }
    }
    
    public async Task CancelPaymentIntentForProjectAsync(string paymentIntentId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Canceling payment intent {PaymentIntentId}", paymentIntentId);

        if (string.IsNullOrEmpty(paymentIntentId)) 
        {
            logger.LogWarning("Payment intent ID not provided");
            
            throw new NotFoundException("This project does not have an attached Payment Intent.");
        }
        
        try
        {
            logger.LogInformation("Retrieving payment intent {PaymentIntentId}", paymentIntentId);
            
            var paymentIntent = await _paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

            if (paymentIntent is null)
            {
                logger.LogError("Payment intent {PaymentIntentId} not found", paymentIntentId);
                
                throw new NotFoundException($"Payment Intent with ID '{paymentIntentId}' not found.");
            }

            if (paymentIntent.Status != "requires_capture" && paymentIntent.Status != "requires_payment_method")
            {
                logger.LogWarning("Payment intent {PaymentIntentId} cannot be canceled in state: {Status}", 
                    paymentIntentId, paymentIntent.Status);
                
                throw new BadRequestException($"Payment Intent with ID '{paymentIntentId}' cannot be canceled in its current state.");
            }
            
            logger.LogInformation("Canceling payment intent {PaymentIntentId}", paymentIntentId);
            
            await _paymentIntentService.CancelAsync(paymentIntentId, cancellationToken: cancellationToken);
            
            logger.LogInformation("Payment intent {PaymentIntentId} canceled successfully", paymentIntentId);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error canceling payment intent: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error canceling payment intent {PaymentIntentId}", paymentIntentId);
            
            throw new BadRequestException($"Could not cancel Payment Intent with ID '{paymentIntentId}'.");
        }
    }
}