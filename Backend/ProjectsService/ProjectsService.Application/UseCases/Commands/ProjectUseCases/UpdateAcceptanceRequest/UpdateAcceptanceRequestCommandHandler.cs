using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;

public class UpdateAcceptanceRequestCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<UpdateAcceptanceRequestCommandHandler> logger) : IRequestHandler<UpdateAcceptanceRequestCommand>
{
    public async Task Handle(UpdateAcceptanceRequestCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating acceptance request for project {ProjectId}", request.ProjectId);

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
        
        if (project.FreelancerId != userId)
        {
            logger.LogWarning("User {UserId} attempted to update acceptance for project {ProjectId} without permission", 
                userId, request.ProjectId);
            
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }
        
        if (project.Lifecycle.Status != ProjectStatus.InProgress && 
            project.Lifecycle.Status != ProjectStatus.Expired)
        {
            logger.LogWarning("Invalid project status {Status} for acceptance request", project.Lifecycle.Status);
            
            throw new BadRequestException("Current project status do not allow you to send acceptance request");
        }
        
        logger.LogInformation("Setting acceptance requested for project {ProjectId}", request.ProjectId);
        
        project.Lifecycle.AcceptanceRequested = true;
        project.Lifecycle.Status = ProjectStatus.PendingForReview;
        
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully updated acceptance request for project {ProjectId}", request.ProjectId);
    }
}