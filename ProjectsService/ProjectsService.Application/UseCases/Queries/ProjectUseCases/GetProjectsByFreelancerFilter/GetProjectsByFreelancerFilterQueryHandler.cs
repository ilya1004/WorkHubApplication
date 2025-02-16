using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.ProjectSpecifications;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;

public class GetProjectsByFreelancerFilterQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetProjectsByFreelancerFilterQuery, PaginatedResultModel<Project>>
{
    public async Task<PaginatedResultModel<Project>> Handle(GetProjectsByFreelancerFilterQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var specification = new GetProjectsByFreelancerFilterSpecification(
            request.FreelancerId,
            request.ProjectStatus,
            request.EmployerId,
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