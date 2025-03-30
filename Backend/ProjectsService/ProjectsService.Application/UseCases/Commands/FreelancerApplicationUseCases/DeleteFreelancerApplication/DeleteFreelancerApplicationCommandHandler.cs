using ProjectsService.Application.Constants;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;

public class DeleteFreelancerApplicationCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<DeleteFreelancerApplicationCommandHandler> logger) : IRequestHandler<DeleteFreelancerApplicationCommand>
{
    public async Task Handle(DeleteFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting freelancer application {ApplicationId}", request.ApplicationId);

        var freelancerApplication = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.ApplicationId, cancellationToken);
        
        if (freelancerApplication is null)
        {
            logger.LogWarning("Freelancer application {ApplicationId} not found", request.ApplicationId);
            
            throw new NotFoundException($"Freelancer Application with ID '{request.ApplicationId}' not found");
        }
        
        var userId = userContext.GetUserId();
        var isResourceOwned = userId == freelancerApplication.FreelancerId;
        var isAdmin = userContext.GetUserRole() == AppRoles.AdminRole;

        if (!isResourceOwned && !isAdmin)
        {
            logger.LogWarning("User {UserId} attempted to delete application {ApplicationId} without permission", 
                userId, request.ApplicationId);
            
            throw new ForbiddenException($"You do not have access to Freelancer Application with ID '{request.ApplicationId}'");
        }
        
        logger.LogInformation("Deleting application {ApplicationId} by user {UserId}", request.ApplicationId, userId);
        
        await unitOfWork.FreelancerApplicationCommandsRepository.DeleteAsync(freelancerApplication, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully deleted freelancer application {ApplicationId}", request.ApplicationId);
    }
}