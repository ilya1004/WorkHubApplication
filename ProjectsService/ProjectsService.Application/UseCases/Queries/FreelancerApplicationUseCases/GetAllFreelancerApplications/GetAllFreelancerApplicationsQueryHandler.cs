using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetAllFreelancerApplications;

public class GetAllFreelancerApplicationsQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetAllFreelancerApplicationsQuery, PaginatedResultModel<FreelancerApplication>>
{
    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(GetAllFreelancerApplicationsQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var applications = await unitOfWork.FreelancerApplicationQueriesRepository.PaginatedListAllAsync(
            offset,
            request.PageSize,
            cancellationToken);
        
        var applicationsCount = await unitOfWork.FreelancerApplicationQueriesRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<FreelancerApplication>
        {
            Items = applications.ToList(),
            TotalCount = applicationsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}