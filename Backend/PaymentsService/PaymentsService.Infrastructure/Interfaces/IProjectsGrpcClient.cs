namespace PaymentsService.Infrastructure.Interfaces;

public interface IProjectsGrpcClient
{
    Task<ProjectDto> GetProjectByIdAsync(string id, CancellationToken cancellationToken);
    Task SaveProjectPaymentIntentAsync(string id, string paymentIntentId, CancellationToken cancellationToken);
}