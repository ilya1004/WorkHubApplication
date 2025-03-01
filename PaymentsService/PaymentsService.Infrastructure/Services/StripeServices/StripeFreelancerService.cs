using PaymentsService.Domain.Abstractions.EmployerService;

namespace PaymentsService.Infrastructure.Services.StripeServices;

public class StripeFreelancerService : IFreelancerService
{
    private readonly AccountService _accountService = new();

    public async Task<string?> CreateConnectedAccountAsync(string email)
    {
        var accountOptions = new AccountCreateOptions
        {
            Type = "express",
            Email = email,
            Capabilities = new AccountCapabilitiesOptions
            {
                Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true }
            }
        };

        try
        {
            var account = await _accountService.CreateAsync(accountOptions);
            return account.Id;
        }
        catch (Exception)
        {
            return null;
        }
    }
}