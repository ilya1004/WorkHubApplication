using PaymentsService.Application.Constants;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.Services.StripeAccountsServices;

public class StripeFreelancerAccountsService(IFreelancersGrpcClient freelancersGrpcClient) : IFreelancerAccountsService
{
    private readonly AccountService _accountService = new();
    private readonly BalanceService _balanceService = new();

    public async Task<string?> CreateFreelancerAccountAsync(Guid userId, string email, CancellationToken cancellationToken) 
    {
        var freelancer = await freelancersGrpcClient.GetFreelancerByIdAsync(userId.ToString(), cancellationToken);
        
        if (!string.IsNullOrEmpty(freelancer.StripeAccountId)) 
            throw new AlreadyExistsException("Your account already exists.");

        var accountOptions = new AccountCreateOptions
        {
            Type = "custom",
            Email = email,
            BusinessType = "individual",
            Individual = new AccountIndividualOptions
            {
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                Phone = "+37052345678",
                Address = new AddressOptions
                {
                    City = "Vilnius",
                    Line1 = "Vilnius",
                    Line2 = "Vilnius",
                    Country = "LT",
                    PostalCode = "01100"
                },
                Dob = new DobOptions
                {
                    Day = 1,
                    Month = 1,
                    Year = 1902
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

            return account.Id;
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BadRequestException($"Could not create an account for freelancer with ID '{userId}'. Error: {ex.Message}");
        }
    }

    public async Task<FreelancerAccountModel> GetFreelancerAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var freelancer = await freelancersGrpcClient.GetFreelancerByIdAsync(userId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(freelancer.StripeAccountId)) 
            throw new NotFoundException($"Stripe account with user ID '{userId}' not found.");

        try
        {
            var account = await _accountService.GetAsync(freelancer.StripeAccountId, cancellationToken: cancellationToken);
            var balance = await _balanceService.GetAsync(
                new BalanceGetOptions(),
                new RequestOptions { StripeAccount = freelancer.StripeAccountId },
                cancellationToken);
            
            if (account is null || balance is null)
            {
                throw new NotFoundException($"Stripe account by user ID '{userId}' not found.");
            }

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
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Stripe account with ID '{freelancer.StripeAccountId}' not found or cannot get its balance.");
        }
    }
}