using PaymentsService.Application.Constants;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.Services.StripeAccountsServices;

public class StripeEmployerAccountsService(
    IEmployersGrpcClient employersGrpcClient) : IEmployerAccountsService
{
    private readonly CustomerService _customerService = new();

    public async Task<string?> CreateEmployerAccountAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);

        if (!string.IsNullOrEmpty(employer.EmployerCustomerId)) throw new AlreadyExistsException("You account is already exists.");

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

    public async Task<EmployerAccountModel> GetEmployerAccountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employer = await employersGrpcClient.GetEmployerByIdAsync(userId.ToString(), cancellationToken);

        if (string.IsNullOrEmpty(employer.EmployerCustomerId)) 
            throw new NotFoundException($"Stripe account by user ID '{userId}' not found.");

        try
        {
            var customer = await _customerService.GetAsync(employer.EmployerCustomerId, cancellationToken: cancellationToken);

            if (customer is null)
            {
                throw new NotFoundException($"Stripe account by user ID '{userId}' not found.");
            }

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
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Stripe customer with ID '{employer.EmployerCustomerId}' not found.");
        }
    }
}