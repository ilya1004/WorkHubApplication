using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.RejectFreelancerApplication;

public class RejectFreelancerApplicationCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<RejectFreelancerApplicationCommandHandler> logger) : IRequestHandler<RejectFreelancerApplicationCommand>
{
    public async Task Handle(RejectFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Rejecting freelancer application {ApplicationId} for project {ProjectId}", 
            request.ApplicationId, request.ProjectId);

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
            logger.LogWarning("User {UserId} attempted to access project {ProjectId} without permission", userId, request.ProjectId);
            
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (project.Lifecycle.Status != ProjectStatus.AcceptingApplications)
        {
            logger.LogWarning("Invalid project status {Status} for rejecting applications", project.Lifecycle.Status);
            
            throw new BadRequestException("You can reject applications to this project only during accepting applications stage");
        }
        
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.ApplicationId,
            cancellationToken);

        if (freelancerApplication is null)
        {
            logger.LogWarning("Freelancer application {ApplicationId} not found", request.ApplicationId);
            
            throw new NotFoundException($"Freelancer application with ID '{request.ApplicationId}' not found");
        }
        
        if (freelancerApplication.Status != ApplicationStatus.Accepted)
        {
            logger.LogWarning("Freelancer application {ApplicationId} has invalid status {Status}", 
                request.ApplicationId, freelancerApplication.Status);
            
            throw new BadRequestException("Freelancer application status is not accepted");
        }

        logger.LogInformation("Rejecting application {ApplicationId}", request.ApplicationId);
        
        freelancerApplication.Status = ApplicationStatus.Pending;

        await unitOfWork.FreelancerApplicationCommandsRepository.UpdateAsync(freelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully rejected freelancer application {ApplicationId} for project {ProjectId}", 
            request.ApplicationId, request.ProjectId);
    }
}