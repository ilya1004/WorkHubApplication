using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceStatus;

public class UpdateAcceptanceStatusCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<UpdateAcceptanceStatusCommandHandler> logger) : IRequestHandler<UpdateAcceptanceStatusCommand>
{
    public async Task Handle(UpdateAcceptanceStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating acceptance status for project {ProjectId}", request.ProjectId);

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

        if (project.EmployerId != userId)
        {
            logger.LogWarning("User {UserId} attempted to update acceptance status for project {ProjectId} without permission", 
                userId, request.ProjectId);
            
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (!project.Lifecycle.AcceptanceRequested)
        {
            logger.LogWarning("Acceptance not requested for project {ProjectId}", request.ProjectId);
            
            throw new BadRequestException("Project acceptance is not requested yet");
        }

        if (project.Lifecycle.Status != ProjectStatus.InProgress && 
            project.Lifecycle.Status != ProjectStatus.Expired)
        {
            logger.LogWarning("Invalid project status {Status} for acceptance update", project.Lifecycle.Status);
            
            throw new BadRequestException("Current project status do not allow you to update acceptance status");
        }
        
        if (request.IsAcceptanceConfirmed)
        {
            logger.LogInformation("Confirming acceptance for project {ProjectId}", request.ProjectId);
            
            project.Lifecycle.AcceptanceConfirmed = true;
            project.Lifecycle.Status = ProjectStatus.Completed;
        }
        else
        {
            logger.LogInformation("Rejecting acceptance for project {ProjectId}", request.ProjectId);
            
            project.Lifecycle.AcceptanceRequested = false;
            project.Lifecycle.AcceptanceConfirmed = false;
        }
        
        await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully updated acceptance status for project {ProjectId}", request.ProjectId);
    }
}