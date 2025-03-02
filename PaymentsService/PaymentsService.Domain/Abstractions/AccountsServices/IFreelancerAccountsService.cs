using PaymentsService.Domain.DTOs;

namespace PaymentsService.Domain.Abstractions.AccountsServices;

public interface IFreelancerAccountsService
{
    Task<string?> CreateFreelancerAccountAsync(Guid userId, string email);
    Task<FreelancerAccountDto?> GetFreelancerAccountAsync(Guid userId);
}