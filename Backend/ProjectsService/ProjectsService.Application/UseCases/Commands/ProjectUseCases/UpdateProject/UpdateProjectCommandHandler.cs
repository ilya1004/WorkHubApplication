using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;

public class UpdateProjectCommandHandler(
    IUnitOfWork unitOfWork, 
    IMapper mapper,
    IUserContext userContext,
    ILogger<UpdateProjectCommandHandler> logger) : IRequestHandler<UpdateProjectCommand>
{
    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating project {ProjectId}", request.ProjectId);

        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found", request.ProjectId);
            
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }
        
        var userId = userContext.GetUserId();
        
        if (project.EmployerId != userId)
        {
            logger.LogWarning("User {UserId} attempted to update project {ProjectId} without permission", userId, request.ProjectId);
            
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }
        
        var lifecycle = await unitOfWork.LifecycleQueriesRepository.FirstOrDefaultAsync(
            l => l.ProjectId == project.Id, cancellationToken);
        
        if (lifecycle is null)
        {
            logger.LogWarning("Lifecycle not found for project {ProjectId}", project.Id);
            
            throw new NotFoundException($"Project lifecycle with ProjectId '{project.Id}' not found");
        }
        
        if (lifecycle.Status != ProjectStatus.Published)
        {
            logger.LogWarning("Invalid project status {Status} for update", lifecycle.Status);
            
            throw new BadRequestException("You cannot edit this project after the start of accepting applications");
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
        
        mapper.Map(request.Project, project);
        mapper.Map(request.Lifecycle, lifecycle);
        
        logger.LogInformation("Saving project {ProjectId} updates", request.ProjectId);
        
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.LifecycleCommandsRepository.UpdateAsync(lifecycle, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully updated project {ProjectId}", request.ProjectId);
    }
}