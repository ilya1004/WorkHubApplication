using PaymentsService.Domain.Models;

namespace PaymentsService.Domain.Abstractions.AccountsServices;

public interface IFreelancerAccountsService
{
    Task<string> CreateFreelancerAccountAsync(Guid userId, string email, CancellationToken cancellationToken);
    Task<FreelancerAccountModel> GetFreelancerAccountAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<FreelancerAccountModel>> GetAllFreelancerAccountsAsync(CancellationToken cancellationToken = default);
}