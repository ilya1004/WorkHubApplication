using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetAllProjects;

public class GetAllProjectsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllProjectsQuery, PaginatedResultModel<Project>>
{
    public async Task<PaginatedResultModel<Project>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;
        
        var projects = await unitOfWork.ProjectQueriesRepository.PaginatedListAllAsync(
            offset, 
            request.PageSize, 
            cancellationToken,
            p => p.Lifecycle,
            p => p.Category!);

        var projectsCount = await unitOfWork.ProjectQueriesRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<Project>
        {
            Items = projects.ToList(),
            TotalCount = projectsCount,
            PageSize = request.PageSize,
            PageNo = request.PageNo
        };
    }
}