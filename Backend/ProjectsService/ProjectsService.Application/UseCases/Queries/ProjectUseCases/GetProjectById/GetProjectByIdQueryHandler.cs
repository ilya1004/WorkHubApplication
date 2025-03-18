namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;

public class GetProjectByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetProjectByIdQuery, Project>
{
    public async Task<Project> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.Id, 
            cancellationToken, 
            p => p.Lifecycle, 
            p => p.Category!);

        if (project is null)
        {
            throw new NotFoundException($"Project with ID '{request.Id}' not found");
        }

        return project;
    }
}