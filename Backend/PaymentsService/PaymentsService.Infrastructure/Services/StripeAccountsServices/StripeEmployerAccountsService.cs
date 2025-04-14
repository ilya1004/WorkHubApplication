using PaymentsService.Application.Constants;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.Services.StripeAccountsServices;

public class StripeEmployerAccountsService(
    IEmployersGrpcClient employersGrpcClient,
    ILogger<StripeEmployerAccountsService> logger) : IEmployerAccountsService
{
    private readonly CustomerService _customerService = new();

    public async Task<string> CreateEmployerAccountAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating Stripe employer account for user {UserId} with email {Email}", userId, email);

        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);

        if (!string.IsNullOrEmpty(employer.EmployerCustomerId))
        {
            logger.LogWarning("Employer account already exists for user {UserId}", userId);
            
            throw new AlreadyExistsException("Your account already exists.");
        }

        var options = new CustomerCreateOptions
        {
            Email = email,
            Metadata = new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "Role", AppRoles.EmployerRole }
            }
        };

        try
        {
            var customer = await _customerService.CreateAsync(options, cancellationToken: cancellationToken);

            if (customer is null)
            {
                logger.LogError("Failed to create Stripe customer for user {UserId}", userId);
            
                throw new BadRequestException($"Stripe account by user ID '{userId}' is not created.");
            }
            
            logger.LogInformation("Successfully created Stripe customer {CustomerId} for user {UserId}", customer.Id, userId);
            
            return customer.Id;
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error while creating account for user {UserId}: {ErrorMessage}", userId, ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating Stripe account for employer {UserId}", userId);
            
            throw new BadRequestException($"Could not create an account for employer with ID '{userId}'.");
        }
    }

    public async Task<EmployerAccountModel> GetEmployerAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Stripe employer account for user {UserId}", userId);

        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
        {
            logger.LogWarning("Stripe account not found for user {UserId}", userId);
            
            throw new NotFoundException($"Stripe account by user ID '{userId}' not found.");
        }

        try
        {
            var customer = await _customerService.GetAsync(employer.EmployerCustomerId, cancellationToken: cancellationToken);

            if (customer is null)
            {
                logger.LogError("Stripe customer {CustomerId} not found for user {UserId}", employer.EmployerCustomerId, userId);
                
                throw new NotFoundException($"Stripe account by user ID '{userId}' not found.");
            }

            logger.LogInformation("Successfully retrieved Stripe account {CustomerId} for user {UserId}", customer.Id, userId);
            
            return new EmployerAccountModel
            {
                Id = employer.EmployerCustomerId,
                OwnerEmail = customer.Email,
                Currency = customer.Currency,
                Balance = customer.Balance
            };
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error while getting account {CustomerId}: {ErrorMessage}", employer.EmployerCustomerId, ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Stripe account {CustomerId}", employer.EmployerCustomerId);
            
            throw new BadRequestException($"Error getting Stripe account with ID '{employer.EmployerCustomerId}'.");
        }
    }
    
    public async Task<IEnumerable<EmployerAccountModel>> GetAllEmployerAccountsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving all Stripe employer accounts");

        try
        {
            var options = new CustomerListOptions
            {
                Limit = 100
            };
            
            var customers = await _customerService.ListAsync(options, cancellationToken: cancellationToken);

            var accounts = customers.Select(customer => 
                new EmployerAccountModel 
                {
                    Id = customer.Id,
                    OwnerEmail = customer.Email,
                    Currency = customer.Currency,
                    Balance = customer.Balance
                }).ToList();

            logger.LogInformation("Successfully retrieved {Count} employer accounts", accounts.Count);

            return accounts;
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error while retrieving employer accounts: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving employer accounts");
            
            throw new BadRequestException("Could not retrieve employer accounts.");
        }
    }
}