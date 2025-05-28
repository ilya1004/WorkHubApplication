namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;

public class GetProjectByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetProjectByIdQueryHandler> logger) : IRequestHandler<GetProjectByIdQuery, Project>
{
    public async Task<Project> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting project by ID: {ProjectId}", request.Id);

        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.Id,
            cancellationToken,
            p => p.Lifecycle, 
            p => p.Category!,
            p => p.FreelancerApplications);

        if (project is null)
        {
            logger.LogWarning("Project with ID {ProjectId} not found", request.Id);
            
            throw new NotFoundException($"Project with ID '{request.Id}' not found");
        }

        logger.LogInformation("Successfully retrieved project with ID: {ProjectId}", request.Id);
        
        return project;
    }
}