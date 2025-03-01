using PaymentsService.Domain.DTOs;

namespace PaymentsService.Domain.Abstractions.EmployerService;

public interface IEmployerService
{
    Task<string?> CreateEmployerAccountAsync(Guid userId, string companyName, string email);
    Task<EmployerAccountDto?> GetEmployerAccountAsync(Guid userId);
}