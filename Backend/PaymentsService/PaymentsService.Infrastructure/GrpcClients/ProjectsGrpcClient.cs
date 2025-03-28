using PaymentsService.Infrastructure.Interfaces;
using Projects;

namespace PaymentsService.Infrastructure.GrpcClients;

public class ProjectsGrpcClient(Projects.Projects.ProjectsClient client, IMapper mapper) : IProjectsGrpcClient
{
    public async Task<ProjectDto> GetProjectByIdAsync(string id, CancellationToken cancellationToken)
    {
        var response = await client.GetProjectByIdAsync(new GetProjectByIdRequest { Id = id }, cancellationToken: cancellationToken);
        
        var projectDto = mapper.Map<ProjectDto>(response);

        return projectDto;
    }
}