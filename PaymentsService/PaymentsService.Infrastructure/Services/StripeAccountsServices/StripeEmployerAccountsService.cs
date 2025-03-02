using PaymentsService.Applications.Constants;
using PaymentsService.Domain.Abstractions.AccountsServices;

namespace PaymentsService.Infrastructure.Services.StripeAccountsServices;

public class StripeEmployerAccountsService : IEmployerAccountsService
{
    private readonly CustomerService _customerService = new();
    public async Task<string?> CreateEmployerAccountAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
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
            return customer.Id;
        }
        catch (Exception)
        {
            throw new BadRequestException($"Could not create an account for employer with ID '{userId}'.");
        }
    }
    
    public async Task<EmployerAccountDto?> GetEmployerAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var stripeAccountId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC
        
        if (stripeAccountId is null || string.IsNullOrEmpty(stripeAccountId))
        {
            throw new NotFoundException($"Stripe account with ID '{stripeAccountId}' not found.");
        }

        try
        {
            var customer = await _customerService.GetAsync(stripeAccountId, cancellationToken: cancellationToken);

            return new EmployerAccountDto
            {
                Id = stripeAccountId,
                OwnerEmail = customer.Email,
                Currency = customer.Currency,
                Balance = customer.Balance,
            };
        }
        catch
        {
            throw new BadRequestException($"Stripe customer with ID '{stripeAccountId}' not found.");
        }
    }
}