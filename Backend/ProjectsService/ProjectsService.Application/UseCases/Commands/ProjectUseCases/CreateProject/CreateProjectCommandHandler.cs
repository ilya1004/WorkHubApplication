using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;

public class CreateProjectCommandHandler(
    IUnitOfWork unitOfWork, 
    IMapper mapper,
    IUserContext userContext) : IRequestHandler<CreateProjectCommand>
{
    public async Task Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
            
        var existingProject = await unitOfWork.ProjectQueriesRepository.FirstOrDefaultAsync(
            p => p.Title == request.Project.Title && p.EmployerId == userId, cancellationToken);

        if (existingProject is not null)
        {
            throw new AlreadyExistsException($"Project with title '{request.Project.Title}' already exists with this employer.");
        }

        if (request.Project.CategoryId.HasValue)
        {
            var isCategoryExists = await unitOfWork.CategoryQueriesRepository.AnyAsync(
                c => c.Id == request.Project.CategoryId, cancellationToken);

            if (!isCategoryExists)
            {
                throw new NotFoundException($"Category with ID '{request.Project.CategoryId}' not found");
            }
        }

        var project = mapper.Map<Project>(request.Project);
        project.EmployerId = userId;
        
        await unitOfWork.ProjectCommandsRepository.AddAsync(project, cancellationToken);
        
        var lifecycle = mapper.Map<Lifecycle>(request);
        lifecycle.ProjectId = project.Id;

        await unitOfWork.LifecycleCommandsRepository.AddAsync(lifecycle, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}