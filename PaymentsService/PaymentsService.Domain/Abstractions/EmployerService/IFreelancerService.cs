namespace PaymentsService.Domain.Abstractions.EmployerService;

public interface IFreelancerService
{
    Task<string?> CreateConnectedAccountAsync(string email);
}