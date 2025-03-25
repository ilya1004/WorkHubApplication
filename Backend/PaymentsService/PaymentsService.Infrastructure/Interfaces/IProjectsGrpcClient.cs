namespace PaymentsService.Infrastructure.Interfaces;

public interface IProjectsGrpcClient
{
    Task<ProjectDto> GetProjectByIdAsync(string id, CancellationToken cancellationToken);
}