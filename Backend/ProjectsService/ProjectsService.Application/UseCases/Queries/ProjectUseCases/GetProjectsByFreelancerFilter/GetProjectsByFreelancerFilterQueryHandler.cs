using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.ProjectSpecifications;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;

public class GetProjectsByFreelancerFilterQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<GetProjectsByFreelancerFilterQueryHandler> logger) : IRequestHandler<GetProjectsByFreelancerFilterQuery, PaginatedResultModel<Project>>
{
    public async Task<PaginatedResultModel<Project>> Handle(GetProjectsByFreelancerFilterQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("User {UserId} getting freelancer filtered projects with filters: {@Filters}", 
            userId, request);

        var offset = (request.PageNo - 1) * request.PageSize;

        var specification = new GetProjectsByFreelancerFilterSpecification(
            userId,
            request.ProjectStatus,
            request.EmployerId,
            offset,
            request.PageSize);

        var projects = await unitOfWork.ProjectQueriesRepository.GetByFilterAsync(specification, cancellationToken);
        
        var projectsCount = await unitOfWork.ProjectQueriesRepository.CountByFilterAsync(specification, cancellationToken);
        
        logger.LogInformation("Retrieved {Count} freelancer filtered projects out of {TotalCount} for user {UserId}", 
            projects.Count, projectsCount, userId);

        return new PaginatedResultModel<Project>
        {
            Items = projects.ToList(),
            TotalCount = projectsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}