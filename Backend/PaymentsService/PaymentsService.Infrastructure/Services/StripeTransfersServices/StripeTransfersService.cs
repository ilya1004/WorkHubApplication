using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.Services.StripeTransfersServices;

public class StripeTransfersService(
    IMapper mapper,
    IEmployersGrpcClient employersGrpcClient,
    IFreelancersGrpcClient freelancersGrpcClient,
    ILogger<StripeTransfersService> logger) : ITransfersService
{
    private readonly ChargeService _chargeService = new();
    private readonly TransferService _transferService = new();
    private readonly PaymentIntentService _paymentIntentService = new();

    public async Task TransferFundsToFreelancer(PaymentIntentModel paymentIntent, Guid projectId, string freelancerStripeAccountId, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Transferring funds for project {ProjectId} to freelancer {FreelancerAccountId}", 
            projectId, freelancerStripeAccountId);

        var transferOptions = new TransferCreateOptions
        {
            Amount = paymentIntent.Amount,
            Currency = paymentIntent.Currency,
            Destination = freelancerStripeAccountId,
            TransferGroup = projectId.ToString(),
            Metadata = new Dictionary<string, string>
            {
                { "project_id", projectId.ToString() }
            }
        };

        try
        {
            logger.LogInformation("Creating transfer with options: {@Options}", transferOptions);
            
            await _transferService.CreateAsync(transferOptions, cancellationToken: cancellationToken);
            
            logger.LogInformation("Funds transferred successfully for project {ProjectId}", projectId);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error transferring funds: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error transferring funds for project {ProjectId}", projectId);
            
            throw new BadRequestException($"Could not create Transfer with Payment intent ID '{paymentIntent.Id}'.");
        }
    }

    public async Task<IEnumerable<ChargeModel>> GetEmployerPaymentsAsync(Guid userId, Guid? projectId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting employer payments for user {UserId}, project {ProjectId}", 
            userId, projectId);

        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);
        
        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
        {
            logger.LogWarning("Employer account not found for user {UserId}", userId);
            
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
        
        var chargeListOptions = new ChargeListOptions
        {
            Customer = employer.EmployerCustomerId
        };
        
        if (projectId is not null)
        {
            chargeListOptions.TransferGroup = projectId.ToString();
        }
        
        try
        {
            logger.LogInformation("Listing charges with options: {@Options}", chargeListOptions);
            
            var charges = await _chargeService.ListAsync(chargeListOptions, cancellationToken: cancellationToken);

            logger.LogInformation("Retrieved {Count} charges for employer {UserId}", charges.Data.Count, userId);
            
            return charges.Data.Select(mapper.Map<ChargeModel>);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error getting employer payments: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting employer payments for user {UserId}", userId);
            
            throw new BadRequestException($"Could not get Payments by employer with ID '{userId}'.");
        }
    }

    public async Task<IEnumerable<ChargeModel>> GetAllEmployerPaymentsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all employer payments");

        var chargeListOptions = new ChargeListOptions
        {
            Limit = 100
        };

        try
        {
            logger.LogInformation("Listing all charges with options: {@Options}", chargeListOptions);

            var charges = await _chargeService.ListAsync(chargeListOptions, cancellationToken: cancellationToken);

            logger.LogInformation("Retrieved {Count} employer charges", charges.Count());

            return charges.Select(mapper.Map<ChargeModel>);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error getting all employer payments: {ErrorMessage}", ex.Message);
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all employer payments");
            throw new BadRequestException("Could not retrieve all employer payments.");
        }
    }

    public async Task<IEnumerable<PaymentIntentModel>> GetEmployerPaymentIntentsAsync(Guid userId, Guid? projectId, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting employer payment intents for user {UserId}, project {ProjectId}", userId, projectId);

        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);
    
        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
        {
            logger.LogWarning("Employer account not found for user {UserId}", userId);
        
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
    
        var paymentIntentListOptions = new PaymentIntentListOptions
        {
            Customer = employer.EmployerCustomerId
        };
    
        try
        {
            logger.LogInformation("Listing payment intents with options: {@Options}", paymentIntentListOptions);
        
            var paymentIntents = await _paymentIntentService.ListAsync(paymentIntentListOptions, cancellationToken: cancellationToken);

            logger.LogInformation("Retrieved {Count} payment intents for employer {UserId}", paymentIntents.Data.Count, userId);
        
            var filteredIntents = projectId.HasValue
                ? paymentIntents.Data.Where(pi => pi.TransferGroup == projectId.ToString())
                : paymentIntents.Data;

            return filteredIntents.Select(mapper.Map<PaymentIntentModel>);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error getting employer payment intents: {ErrorMessage}", ex.Message);
        
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting employer payment intents for user {UserId}", userId);
        
            throw new BadRequestException($"Could not get PaymentIntents by employer with ID '{userId}'.");
        }
    }

    public async Task<IEnumerable<TransferModel>> GetFreelancerTransfersAsync(Guid userId, Guid? projectId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting freelancer transfers for user {UserId}, project {ProjectId}", 
            userId, projectId);

        var freelancer = await freelancersGrpcClient.GetFreelancerByIdAsync(userId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(freelancer.StripeAccountId)) 
        {
            logger.LogWarning("Freelancer account not found for user {UserId}", userId);
            
            throw new NotFoundException($"Stripe account with user ID '{userId}' not found.");
        }

        var options = new TransferListOptions
        {
            Destination = freelancer.StripeAccountId
        };

        if (projectId is not null) 
            options.TransferGroup = projectId.ToString();

        try
        {
            logger.LogInformation("Listing transfers with options: {@Options}", options);
            
            var transfers = await _transferService.ListAsync(options, cancellationToken: cancellationToken);

            logger.LogInformation("Retrieved {Count} transfers for freelancer {UserId}", 
                transfers.Data.Count, userId);
            
            return transfers.Data.Select(mapper.Map<TransferModel>);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error getting freelancer transfers: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting freelancer transfers for user {UserId}", userId);
            
            throw new BadRequestException($"Could not get Payments by freelancer with ID '{userId}'.");
        }
    }

    public async Task<IEnumerable<TransferModel>> GetAllFreelancerTransfersAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all freelancer transfers");

        var transferListOptions = new TransferListOptions
        {
            Limit = 100
        };

        try
        {
            logger.LogInformation("Listing all transfers with options: {@Options}", transferListOptions);

            var transfers = await _transferService.ListAsync(transferListOptions, cancellationToken: cancellationToken);

            logger.LogInformation("Retrieved {Count} freelancer transfers", transfers.Count());

            return transfers.Select(mapper.Map<TransferModel>);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error getting all freelancer transfers: {ErrorMessage}", ex.Message);
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all freelancer transfers");
            throw new BadRequestException("Could not retrieve all freelancer transfers.");
        }
    }
}