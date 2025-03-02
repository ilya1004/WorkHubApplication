using PaymentsService.Applications.Constants;
using PaymentsService.Domain.Abstractions.AccountsServices;

namespace PaymentsService.Infrastructure.Services.StripeAccountsServices;

public class StripeFreelancerAccountsService : IFreelancerAccountsService
{
    private readonly AccountService _accountService = new();
    private readonly BalanceService _balanceService = new();
    public async Task<string?> CreateFreelancerAccountAsync(Guid userId, string email)
    {
        var accountOptions = new AccountCreateOptions
        {
            Type = "express",
            Email = email,
            Capabilities = new AccountCapabilitiesOptions
            {
                Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true }
            },
            Metadata = new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "Role", AppRoles.FreelancerRole }
            }
        };

        try
        {
            var account = await _accountService.CreateAsync(accountOptions);
            return account.Id;
        }
        catch
        {
            throw new BadRequestException($"Could not create an account for freelancer with ID '{userId}'.");
        }
    }
    
    public async Task<FreelancerAccountDto?> GetFreelancerAccountAsync(Guid userId)
    {
        var stripeAccountId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC
        
        if (stripeAccountId is null || string.IsNullOrEmpty(stripeAccountId))
        {
            throw new NotFoundException($"Stripe account with ID '{stripeAccountId}' not found.");
        }

        try
        {
            var account = await _accountService.GetAsync(stripeAccountId);
            var balance = await _balanceService.GetAsync(
                new BalanceGetOptions(), 
                new RequestOptions { StripeAccount = stripeAccountId });

            return new FreelancerAccountDto
            {
                Id = stripeAccountId,
                OwnerEmail = account.Email,
                AccountType = account.Type,
                Country = account.Country,
                Balance = balance.Available.Select(b => 
                    new BalanceAmountDto
                    {
                        Amount = b.Amount, 
                        Currency = b.Currency
                    }),
            };
        }
        catch (Exception ex)
        {
            throw new BadRequestException($"Stripe account with ID '{stripeAccountId}' not found or cannot get its balance.");
        }
    }
}