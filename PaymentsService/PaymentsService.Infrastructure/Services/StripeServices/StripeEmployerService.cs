using PaymentsService.Applications.Constants;
using PaymentsService.Applications.Exceptions;
using PaymentsService.Domain.Abstractions.EmployerService;
using PaymentsService.Domain.DTOs;


namespace PaymentsService.Infrastructure.Services.StripeServices;

public class StripeEmployerService : IEmployerService
{
    private readonly CustomerService _customerService = new();

    public async Task<string?> CreateEmployerAccountAsync(Guid userId, string companyName, string email) // This will be requested from identity service via gRPC
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = companyName,
            Metadata = new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "Role", AppRoles.EmployerRole }
            }
        };

        try
        {
            var customer = await _customerService.CreateAsync(options);
            return customer.Id;
        }
        catch (Exception)
        {
            throw new BadRequestException($"Could not create an account for employer with ID '{userId}'.");
        }
    }
    
    public async Task<EmployerAccountDto?> GetEmployerAccountAsync(Guid userId)
    {
        var stripeAccountId = Guid.NewGuid().ToString(); // It will be requested from identity service via gRPC
        
        if (stripeAccountId is null || string.IsNullOrEmpty(stripeAccountId))
        {
            throw new NotFoundException($"Stripe account with ID '{stripeAccountId}' not found.");
        }

        try
        {
            var customer = await _customerService.GetAsync(stripeAccountId);

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