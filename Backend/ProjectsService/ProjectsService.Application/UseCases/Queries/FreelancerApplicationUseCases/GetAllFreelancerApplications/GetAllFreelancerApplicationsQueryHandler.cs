using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetAllFreelancerApplications;

public class GetAllFreelancerApplicationsQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetAllFreelancerApplicationsQueryHandler> logger) : IRequestHandler<GetAllFreelancerApplicationsQuery, PaginatedResultModel<FreelancerApplication>>
{
    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(GetAllFreelancerApplicationsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all freelancer applications with pagination - Page: {PageNo}, Size: {PageSize}", 
            request.PageNo, request.PageSize);

        var offset = (request.PageNo - 1) * request.PageSize;

        var applications = await unitOfWork.FreelancerApplicationQueriesRepository.PaginatedListAllAsync(
            offset, request.PageSize, cancellationToken, fa => fa.Project);
        
        var applicationsCount = await unitOfWork.FreelancerApplicationQueriesRepository.CountAllAsync(cancellationToken);

        logger.LogInformation("Retrieved {Count} applications out of {TotalCount}", 
            applications.Count, applicationsCount);

        return new PaginatedResultModel<FreelancerApplication>
        {
            Items = applications.ToList(),
            TotalCount = applicationsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}