using PaymentsService.Infrastructure.Interfaces;
using Projects;

namespace PaymentsService.Infrastructure.GrpcClients;

public class ProjectsGrpcClient(
    Projects.Projects.ProjectsClient client, 
    IMapper mapper,
    ILogger<ProjectsGrpcClient> logger) : IProjectsGrpcClient
{
    public async Task<ProjectDto> GetProjectByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Requesting project with ID {ProjectId} from gRPC service", id);
        
        var response = await client.GetProjectByIdAsync(
            new GetProjectByIdRequest { Id = id }, 
            cancellationToken: cancellationToken);
        
        if (response is null)
        {
            logger.LogWarning("Project with ID '{ProjectId}' not found ", id);
            
            throw new NotFoundException($"Project with ID '{id}' not found.");
        }
        
        logger.LogInformation("Successfully received project with ID {ProjectId} from gRPC service", id);

        var projectDto = mapper.Map<ProjectDto>(response);

        return projectDto;
    }
}