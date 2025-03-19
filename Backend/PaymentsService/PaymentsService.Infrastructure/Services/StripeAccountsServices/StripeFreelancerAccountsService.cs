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

        if (freelancer.StripeAccountId is not null) throw new AlreadyExistsException("You account is already exists.");

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
            var account = await _accountService.CreateAsync(accountOptions, cancellationToken: cancellationToken);
            return account.Id;
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not create an account for freelancer with ID '{userId}'.");
        }
    }

    public async Task<FreelancerAccountModel?> GetFreelancerAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var freelancer = await freelancersGrpcClient.GetFreelancerByIdAsync(userId.ToString(), cancellationToken);

        if (freelancer.StripeAccountId is null)
            throw new NotFoundException($"Stripe account with user ID '{userId}' not found.");

        try
        {
            var account = await _accountService.GetAsync(freelancer.StripeAccountId, cancellationToken: cancellationToken);
            var balance = await _balanceService.GetAsync(
                new BalanceGetOptions(),
                new RequestOptions { StripeAccount = freelancer.StripeAccountId },
                cancellationToken);

            return new FreelancerAccountModel
            {
                Id = freelancer.StripeAccountId,
                OwnerEmail = account.Email,
                AccountType = account.Type,
                Country = account.Country,
                Balance = balance.Available.Select(b =>
                    new BalanceAmountModel
                    {
                        Amount = b.Amount,
                        Currency = b.Currency
                    })
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