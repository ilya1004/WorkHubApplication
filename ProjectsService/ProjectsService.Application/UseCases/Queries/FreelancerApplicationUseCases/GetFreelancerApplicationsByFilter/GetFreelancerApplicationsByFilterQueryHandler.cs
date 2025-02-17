using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.FreelancerApplicationSpecifications;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;

public class GetFreelancerApplicationsByFilterQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetFreelancerApplicationsByFilterQuery, PaginatedResultModel<FreelancerApplication>>
{
    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(GetFreelancerApplicationsByFilterQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var specification = new GetFreelancerApplicationsByFilterSpecification(
            request.FreelancerId,
            request.StartDate,
            request.EndDate,
            request.ApplicationStatus,
            offset,
            request.PageSize);

        var applications = await unitOfWork.FreelancerApplicationQueriesRepository.GetByFilterAsync(
            specification, cancellationToken);

        var applicationsCount = await unitOfWork.FreelancerApplicationQueriesRepository.CountByFilterAsync(
            specification, cancellationToken);

        return new PaginatedResultModel<FreelancerApplication>
        {
            Items = applications.ToList(),
            TotalCount = applicationsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}