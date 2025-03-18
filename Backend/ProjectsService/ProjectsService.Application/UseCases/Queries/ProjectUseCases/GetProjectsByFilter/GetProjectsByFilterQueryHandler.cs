using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.ProjectSpecifications;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFilter;

public class GetProjectsByFilterQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetProjectsByFilterQuery, PaginatedResultModel<Project>>
{
    public async Task<PaginatedResultModel<Project>> Handle(GetProjectsByFilterQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var specification = new GetProjectsByFilterSpecification(
            request.Title,
            request.BudgetFrom,
            request.BudgetTo,
            request.CategoryId,
            request.EmployerId,
            request.ProjectStatus,
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