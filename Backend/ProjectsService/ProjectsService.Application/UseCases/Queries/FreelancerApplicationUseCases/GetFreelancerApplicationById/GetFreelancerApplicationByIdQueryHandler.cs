using ProjectsService.Application.Constants;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationById;

public class GetFreelancerApplicationByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<GetFreelancerApplicationByIdQueryHandler> logger) : IRequestHandler<GetFreelancerApplicationByIdQuery, FreelancerApplication>
{
    public async Task<FreelancerApplication> Handle(GetFreelancerApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting freelancer application by ID: {ApplicationId}", request.ApplicationId);

        var application = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.ApplicationId, cancellationToken, fa => fa.Project);
        
        if (application is null)
        {
            logger.LogWarning("Freelancer application with ID {ApplicationId} not found", request.ApplicationId);
            
            throw new NotFoundException($"Freelancer Application with ID '{request.ApplicationId}' not found");
        }

        var userId = userContext.GetUserId();
        var isResourceOwned = userId == application.FreelancerId;
        var isAdmin = userContext.GetUserRole() == AppRoles.AdminRole;
        
        if (!isResourceOwned && !isAdmin)
        {
            logger.LogWarning("User {UserId} attempted to access application {ApplicationId} without permission", userId, request.ApplicationId);
            
            throw new ForbiddenException($"You do not have access to Freelancer Application with ID '{request.ApplicationId}'");
        }
        
        logger.LogInformation("Successfully retrieved freelancer application with ID: {ApplicationId}", request.ApplicationId);
        
        return application;
    }
}