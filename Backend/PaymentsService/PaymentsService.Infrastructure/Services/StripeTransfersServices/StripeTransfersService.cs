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

    public async Task TransferFundsToFreelancer(PaymentIntentModel paymentIntent, Guid projectId,
        string freelancerStripeAccountId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Transferring funds for project {ProjectId} to freelancer {FreelancerAccountId}", 
            projectId, freelancerStripeAccountId);

        var transferOptions = new TransferCreateOptions
        {
            Amount = paymentIntent.Amount,
            Currency = paymentIntent.Currency,
            Destination = freelancerStripeAccountId,
            TransferGroup = projectId.ToString()
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

    public async Task<IEnumerable<ChargeModel>> GetEmployerPaymentsAsync(
        Guid userId, Guid? projectId, CancellationToken cancellationToken)
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
            chargeListOptions.TransferGroup = projectId.ToString();

        try
        {
            logger.LogInformation("Listing charges with options: {@Options}", chargeListOptions);
            
            var charges = await _chargeService.ListAsync(chargeListOptions, cancellationToken: cancellationToken);

            logger.LogInformation("Retrieved {Count} charges for employer {UserId}", 
                charges.Data.Count, userId);
            
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

    public async Task<IEnumerable<TransferModel>> GetFreelancerTransfersAsync(
        Guid userId, Guid? projectId, CancellationToken cancellationToken)
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
}