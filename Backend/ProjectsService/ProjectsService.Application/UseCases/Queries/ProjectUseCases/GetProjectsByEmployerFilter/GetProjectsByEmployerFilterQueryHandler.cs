using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.ProjectSpecifications;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;

public class GetProjectsByEmployerFilterQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<GetProjectsByEmployerFilterQueryHandler> logger) : IRequestHandler<GetProjectsByEmployerFilterQuery, PaginatedResultModel<Project>>
{
    public async Task<PaginatedResultModel<Project>> Handle(GetProjectsByEmployerFilterQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("User {UserId} getting employer filtered projects with filters: {@Filters}", 
            userId, request);

        var offset = (request.PageNo - 1) * request.PageSize;

        var specification = new GetProjectsByEmployerFilterSpecification(
            userId,
            request.UpdatedAtStartDate,
            request.UpdatedAtEndDate,
            request.ProjectStatus,
            request.AcceptanceRequestedAndNotConfirmed,
            offset,
            request.PageSize);

        var projects = await unitOfWork.ProjectQueriesRepository.GetByFilterAsync(specification, cancellationToken);
        
        var projectsCount = await unitOfWork.ProjectQueriesRepository.CountByFilterAsync(specification, cancellationToken);
        
        logger.LogInformation("Retrieved {Count} employer filtered projects out of {TotalCount} for user {UserId}", 
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