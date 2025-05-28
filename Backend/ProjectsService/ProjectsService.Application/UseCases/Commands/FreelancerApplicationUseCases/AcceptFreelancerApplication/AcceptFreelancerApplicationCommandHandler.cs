using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.AcceptFreelancerApplication;

public class AcceptFreelancerApplicationCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<AcceptFreelancerApplicationCommandHandler> logger) : IRequestHandler<AcceptFreelancerApplicationCommand>
{
    public async Task Handle(AcceptFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting to accept freelancer application {ApplicationId} for project {ProjectId}", 
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

        if (project.FreelancerId is not null)
        {
            logger.LogWarning("Project {ProjectId} already has freelancer assigned", request.ProjectId);
            
            throw new BadRequestException("This project already has freelancer to work on it");
        }

        if (project.Lifecycle.Status != ProjectStatus.AcceptingApplications)
        {
            logger.LogWarning("Invalid project status {Status} for accepting applications", project.Lifecycle.Status);
            
            throw new BadRequestException("You can accept applications to this project only during accepting applications stage");
        }

        var hasAcceptedApplication = await unitOfWork.FreelancerApplicationQueriesRepository.AnyAsync(
            fa => fa.Status == ApplicationStatus.Accepted, cancellationToken);

        if (hasAcceptedApplication)
        {
            logger.LogWarning("You already has accepted freelancer application to this project with ID '{ProjectId}'", project.Id);
            
            throw new BadRequestException($"You already has accepted freelancer application to this project with ID '{project.Id}'");
        }
        
        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.ApplicationId,
            cancellationToken);

        if (freelancerApplication is null)
        {
            logger.LogWarning("Freelancer application {ApplicationId} not found", request.ApplicationId);
            
            throw new NotFoundException($"Freelancer application with ID '{request.ApplicationId}' not found");
        }

        if (freelancerApplication.Status != ApplicationStatus.Pending)
        {
            logger.LogWarning("Freelancer application {ApplicationId} has invalid status {Status}", 
                request.ApplicationId, freelancerApplication.Status);
            
            throw new BadRequestException("Freelancer application status is not pending");
        }

        freelancerApplication.Status = ApplicationStatus.Accepted;

        await unitOfWork.FreelancerApplicationCommandsRepository.UpdateAsync(freelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully accepted freelancer application {ApplicationId} for project {ProjectId}", 
            request.ApplicationId, request.ProjectId);
    }
}