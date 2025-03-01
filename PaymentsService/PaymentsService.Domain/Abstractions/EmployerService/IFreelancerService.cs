using PaymentsService.Domain.DTOs;

namespace PaymentsService.Domain.Abstractions.EmployerService;

public interface IFreelancerService
{
    Task<string?> CreateConnectedAccountAsync(Guid userId, string email);
    Task<FreelancerAccountDto?> GetFreelancerAccountAsync(Guid userId);
}