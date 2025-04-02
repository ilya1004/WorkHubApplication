using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;

public class CreateProjectCommandHandler(
    IUnitOfWork unitOfWork, 
    IMapper mapper,
    IUserContext userContext,
    ILogger<CreateProjectCommandHandler> logger) : IRequestHandler<CreateProjectCommand>
{
    public async Task Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("User {UserId} creating new project with title '{Title}'", userId, request.Project.Title);
            
        var existingProject = await unitOfWork.ProjectQueriesRepository.FirstOrDefaultAsync(
            p => p.Title == request.Project.Title && p.EmployerId == userId, cancellationToken);

        if (existingProject is not null)
        {
            logger.LogWarning("Project with title '{Title}' already exists for user {UserId}", request.Project.Title, userId);
            
            throw new AlreadyExistsException($"Project with title '{request.Project.Title}' already exists with this employer.");
        }

        if (request.Project.CategoryId.HasValue)
        {
            logger.LogInformation("Checking category {CategoryId} existence", request.Project.CategoryId);
            
            var isCategoryExists = await unitOfWork.CategoryQueriesRepository.AnyAsync(
                c => c.Id == request.Project.CategoryId, cancellationToken);

            if (!isCategoryExists)
            {
                logger.LogWarning("Category {CategoryId} not found", request.Project.CategoryId);
                
                throw new NotFoundException($"Category with ID '{request.Project.CategoryId}' not found");
            }
        }

        var project = mapper.Map<Project>(request.Project);
        project.EmployerId = userId;
        
        logger.LogInformation("Adding new project {ProjectId}", project.Id);
        
        await unitOfWork.ProjectCommandsRepository.AddAsync(project, cancellationToken);
        
        var lifecycle = mapper.Map<Lifecycle>(request);
        lifecycle.ProjectId = project.Id;

        logger.LogInformation("Adding lifecycle for project {ProjectId}", project.Id);
        
        await unitOfWork.LifecycleCommandsRepository.AddAsync(lifecycle, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully created project {ProjectId}", project.Id);
    }
}