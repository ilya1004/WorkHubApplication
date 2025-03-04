using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.FreelancerApplicationSpecifications;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;

public class GetFreelancerApplicationsByFilterQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<GetFreelancerApplicationsByFilterQuery, PaginatedResultModel<FreelancerApplication>>
{
    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(GetFreelancerApplicationsByFilterQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;
        var userId = userContext.GetUserId();

        var specification = new GetFreelancerApplicationsByFilterSpecification(
            userId,
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