using PaymentsService.Application.Constants;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Infrastructure.DTOs;

namespace PaymentsService.Infrastructure.Services.StripeAccountsServices;

public class StripeEmployerAccountsService : IEmployerAccountsService
{
    private readonly CustomerService _customerService = new();

    public async Task<string?> CreateEmployerAccountAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
        var employer = new EmployerDto(); // This data will be requested from Identity Service via gRPC

        if (employer.EmployerCustomerId is not null) throw new AlreadyExistsException("You account is already exists.");

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
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch (Exception)
        {
            throw new BadRequestException($"Could not create an account for employer with ID '{userId}'.");
        }
    }

    public async Task<EmployerAccountModel?> GetEmployerAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        if (employerCustomerId is null || string.IsNullOrEmpty(employerCustomerId))
            throw new NotFoundException($"Stripe account with ID '{employerCustomerId}' not found.");

        try
        {
            var customer = await _customerService.GetAsync(employerCustomerId, cancellationToken: cancellationToken);

            return new EmployerAccountModel
            {
                Id = employerCustomerId,
                OwnerEmail = customer.Email,
                Currency = customer.Currency,
                Balance = customer.Balance
            };
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Stripe customer with ID '{employerCustomerId}' not found.");
        }
    }
}