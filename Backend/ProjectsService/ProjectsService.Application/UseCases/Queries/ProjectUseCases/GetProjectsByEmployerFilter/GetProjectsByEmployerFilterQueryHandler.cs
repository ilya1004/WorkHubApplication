using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.ProjectSpecifications;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;

public class GetProjectsByEmployerFilterQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<GetProjectsByEmployerFilterQuery, PaginatedResultModel<Project>>
{
    public async Task<PaginatedResultModel<Project>> Handle(GetProjectsByEmployerFilterQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;
        var userId = userContext.GetUserId();

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
        
        return new PaginatedResultModel<Project>
        {
            Items = projects.ToList(),
            TotalCount = projectsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}