using PaymentsService.Domain.Models;

namespace PaymentsService.Domain.Abstractions.AccountsServices;

public interface IEmployerAccountsService
{
    Task<string> CreateEmployerAccountAsync(Guid userId, string email, CancellationToken cancellationToken);
    Task<EmployerAccountModel> GetEmployerAccountAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<EmployerAccountModel>> GetAllEmployerAccountsAsync(CancellationToken cancellationToken = default);
}