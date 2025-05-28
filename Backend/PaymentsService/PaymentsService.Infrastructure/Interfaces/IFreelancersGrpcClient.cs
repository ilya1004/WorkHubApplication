namespace PaymentsService.Infrastructure.Interfaces;

public interface IFreelancersGrpcClient
{
    Task<FreelancerDto> GetFreelancerByIdAsync(string id, CancellationToken cancellationToken);
}