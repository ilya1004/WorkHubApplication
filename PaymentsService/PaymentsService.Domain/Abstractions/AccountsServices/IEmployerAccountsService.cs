using PaymentsService.Domain.DTOs;

namespace PaymentsService.Domain.Abstractions.AccountsServices;

public interface IEmployerAccountsService
{
    Task<string?> CreateEmployerAccountAsync(Guid userId, string email, CancellationToken cancellationToken);
    Task<EmployerAccountDto?> GetEmployerAccountAsync(Guid userId, CancellationToken cancellationToken);
}