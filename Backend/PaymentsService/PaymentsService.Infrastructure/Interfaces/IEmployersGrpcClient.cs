namespace PaymentsService.Infrastructure.Interfaces;

public interface IEmployersGrpcClient
{
    Task<EmployerDto> GetEmployerByIdAsync(string id, CancellationToken cancellationToken);
}