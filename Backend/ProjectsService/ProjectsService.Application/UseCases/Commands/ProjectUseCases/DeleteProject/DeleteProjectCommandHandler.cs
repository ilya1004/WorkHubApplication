using ProjectsService.Application.Constants;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.DeleteProject;

public class DeleteProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<DeleteProjectCommandHandler> logger) : IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting project {ProjectId}", request.ProjectId);

        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(
            request.ProjectId, 
            cancellationToken,
            p => p.Lifecycle);
        
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found", request.ProjectId);
            
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }
        
        var userId = userContext.GetUserId();
        var isResourceOwned = userId == project.EmployerId;
        var isAdmin = userContext.GetUserRole() == AppRoles.AdminRole;

        if (!isResourceOwned && !isAdmin)
        {
            logger.LogWarning("User {UserId} attempted to delete project {ProjectId} without permission", 
                userId, request.ProjectId);
            
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (project.Lifecycle.Status != ProjectStatus.Cancelled)
        {
            logger.LogWarning("Project {ProjectId} must be cancelled before deletion", request.ProjectId);
            
            throw new BadRequestException("You need to cancel the project before its removing");
        }
        
        logger.LogInformation("Deleting project {ProjectId}", request.ProjectId);
        
        await unitOfWork.ProjectCommandsRepository.DeleteAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully deleted project {ProjectId}", request.ProjectId);
    }
}
