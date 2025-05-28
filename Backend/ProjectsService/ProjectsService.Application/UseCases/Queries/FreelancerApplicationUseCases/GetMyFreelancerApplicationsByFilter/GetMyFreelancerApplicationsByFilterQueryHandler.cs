using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.FreelancerApplicationSpecifications;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetMyFreelancerApplicationsByFilter;

public class GetMyFreelancerApplicationsByFilterQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<GetMyFreelancerApplicationsByFilterQueryHandler> logger) : IRequestHandler<GetMyFreelancerApplicationsByFilterQuery, PaginatedResultModel<FreelancerApplication>>
{
    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(GetMyFreelancerApplicationsByFilterQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("User {UserId} getting filtered freelancer applications with filters: {@Filters}", 
            userId, request);

        var offset = (request.PageNo - 1) * request.PageSize;

        var specification = new GetMyFreelancerApplicationsByFilterSpecification(
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

        logger.LogInformation("Retrieved {Count} filtered applications out of {TotalCount} for user {UserId}", 
            applications.Count, applicationsCount, userId);

        return new PaginatedResultModel<FreelancerApplication>
        {
            Items = applications.ToList(),
            TotalCount = applicationsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}