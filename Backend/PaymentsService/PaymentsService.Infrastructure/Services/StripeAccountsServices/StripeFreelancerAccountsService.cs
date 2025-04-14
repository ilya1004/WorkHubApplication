using PaymentsService.Application.Constants;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.Services.StripeAccountsServices;

public class StripeFreelancerAccountsService(
    IFreelancersGrpcClient freelancersGrpcClient,
    ILogger<StripeFreelancerAccountsService> logger) : IFreelancerAccountsService
{
    private readonly AccountService _accountService = new();
    private readonly BalanceService _balanceService = new();

    public async Task<string> CreateFreelancerAccountAsync(Guid userId, string email, CancellationToken cancellationToken) 
    {
        logger.LogInformation("Creating Stripe freelancer account for user {UserId} with email {Email}", userId, email);

        var freelancer = await freelancersGrpcClient.GetFreelancerByIdAsync(userId.ToString(), cancellationToken);
        
        if (!string.IsNullOrEmpty(freelancer.StripeAccountId)) 
        {
            logger.LogWarning("Freelancer account already exists for user {UserId}", userId);
            
            throw new AlreadyExistsException("Your account already exists.");
        }

        var accountOptions = new AccountCreateOptions
        {
            Type = "custom",
            Email = email,
            BusinessType = "individual",
            Country = "LT",
            Individual = new AccountIndividualOptions
            {
                FirstName = "Test",
                LastName = "Freelancer",
                Email = email,
                Phone = "+37061234567",
                Address = new AddressOptions
                {
                    City = "Vilnius",
                    Line1 = "Test Street 123",
                    Country = "LT",
                    PostalCode = "LT-01100"
                },
                Dob = new DobOptions
                {
                    Day = 1,
                    Month = 1,
                    Year = 1990
                },
            },
            BusinessProfile = new AccountBusinessProfileOptions
            {
                Name = "WorkHub",
                Url = "https://www.workhub.me",
                Mcc = "7372"
            },
            Capabilities = new AccountCapabilitiesOptions
            {
                Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true }
            },
            ExternalAccount = new AccountBankAccountOptions
            {
                AccountNumber = "LT121000011101001000",
                Country = "LT",
                Currency = "eur",
            },
            TosAcceptance = new AccountTosAcceptanceOptions
            {
                Date = DateTime.UtcNow,
                Ip = "127.0.0.1"
            },
            Metadata = new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "Role", AppRoles.FreelancerRole }
            }
        };

        try
        {
            var account = await _accountService.CreateAsync(accountOptions, cancellationToken: cancellationToken);
            
            if (account is null)
            {
                logger.LogError("Failed to create Stripe account for user {UserId}", userId);
                
                throw new BadRequestException($"Stripe account by user ID '{userId}' is not created.");
            }

            logger.LogInformation("Successfully created Stripe account {AccountId} for user {UserId}", account.Id, userId);
            
            return account.Id;
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error while creating account for user {UserId}: {ErrorMessage}", userId, ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating Stripe account for freelancer {UserId}", userId);
            
            throw new BadRequestException($"Could not create an account for freelancer with ID '{userId}'. Error: {ex.Message}");
        }
    }

    public async Task<FreelancerAccountModel> GetFreelancerAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Stripe freelancer account for user {UserId}", userId);

        var freelancer = await freelancersGrpcClient.GetFreelancerByIdAsync(userId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(freelancer.StripeAccountId)) 
        {
            logger.LogWarning("Stripe account not found for user {UserId}", userId);
            
            throw new NotFoundException($"Stripe account with user ID '{userId}' not found.");
        }

        try
        {
            var account = await _accountService.GetAsync(freelancer.StripeAccountId, cancellationToken: cancellationToken);
            
            logger.LogInformation("Retrieving balance for account {AccountId}", freelancer.StripeAccountId);
            
            var balance = await _balanceService.GetAsync(
                new BalanceGetOptions(),
                new RequestOptions { StripeAccount = freelancer.StripeAccountId },
                cancellationToken);
            
            if (account is null || balance is null)
            {
                logger.LogError("Stripe account or balance not found for account {AccountId}", freelancer.StripeAccountId);
                
                throw new NotFoundException($"Stripe account by user ID '{userId}' not found.");
            }

            logger.LogInformation("Successfully retrieved Stripe account {AccountId} for user {UserId}", account.Id, userId);
            
            return new FreelancerAccountModel
            {
                Id = freelancer.StripeAccountId,
                OwnerEmail = account.Email,
                AccountType = account.Type,
                Country = account.Country,
                Balance = balance.Available.Where(x => x.Currency == "eur").Sum(x => x.Amount),
            };
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error while getting account {AccountId}: {ErrorMessage}", freelancer.StripeAccountId, ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Stripe account {AccountId}", freelancer.StripeAccountId);
            
            throw new BadRequestException($"Error getting Stripe account {freelancer.StripeAccountId}");
        }
    }
    
    public async Task<IEnumerable<FreelancerAccountModel>> GetAllFreelancerAccountsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving all Stripe freelancer accounts");

        try
        {
            var options = new AccountListOptions
            {
                Limit = 100,
            };

            var accounts = await _accountService.ListAsync(options, cancellationToken: cancellationToken);

            var accountModels = new List<FreelancerAccountModel>();

            foreach (var account in accounts)
            {
                try
                {
                    var balance = await _balanceService.GetAsync(
                        new BalanceGetOptions(),
                        new RequestOptions { StripeAccount = account.Id },
                        cancellationToken);

                    accountModels.Add(new FreelancerAccountModel
                    {
                        Id = account.Id,
                        OwnerEmail = account.Email,
                        AccountType = account.Type,
                        Country = account.Country,
                        Balance = balance.Available.Where(x => x.Currency == "eur").Sum(x => x.Amount)
                    });
                }
                catch (StripeException ex)
                {
                    logger.LogWarning(ex, "Failed to retrieve balance for account {AccountId}: {ErrorMessage}", account.Id, ex.Message);
                }
            }

            logger.LogInformation("Successfully retrieved {Count} freelancer accounts", accountModels.Count);

            return accountModels;
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe error while retrieving freelancer accounts: {ErrorMessage}", ex.Message);
            
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving freelancer accounts");
            
            throw new BadRequestException("Could not retrieve freelancer accounts.");
        }
    }
}