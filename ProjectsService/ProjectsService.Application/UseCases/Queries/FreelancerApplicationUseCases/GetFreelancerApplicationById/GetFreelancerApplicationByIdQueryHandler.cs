using ProjectsService.Application.Constants;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationById;

public class GetFreelancerApplicationByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<GetFreelancerApplicationByIdQuery, FreelancerApplication>
{
    public async Task<FreelancerApplication> Handle(GetFreelancerApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var application = await unitOfWork.FreelancerApplicationQueriesRepository.GetByIdAsync(
            request.ApplicationId, cancellationToken);
        
        if (application is null)
        {
            throw new NotFoundException($"Freelancer Application with ID '{request.ApplicationId}' not found");
        }

        var isResourceOwned = userContext.GetUserId() == application.FreelancerId;
        var isAdmin = userContext.GetUserRole() == AppRoles.AdminRole;
        
        if (!isResourceOwned && !isAdmin)
        {
            throw new ForbiddenException($"You do not have access to Freelancer Application with ID '{request.ApplicationId}'");
        }
        
        return application;
    }
}