using ProjectsService.Application.Constants;
using ProjectsService.Application.Models;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByProjectId;

public class GetFreelancerApplicationsByProjectIdQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<GetFreelancerApplicationsByProjectIdQueryHandler> logger) : IRequestHandler<GetFreelancerApplicationsByProjectIdQuery, PaginatedResultModel<FreelancerApplication>>
{
    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(GetFreelancerApplicationsByProjectIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting freelancer applications for project ID: {ProjectId}", request.ProjectId);

        var project = await unitOfWork.ProjectQueriesRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        
        if (project is null)
        {
            logger.LogWarning("Project with ID {ProjectId} not found", request.ProjectId);
            
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }

        var userId = userContext.GetUserId();
        var isResourceOwner = userId == project.EmployerId;
        var isAdmin = userContext.GetUserRole() == AppRoles.AdminRole;
        
        if (!isResourceOwner && !isAdmin)
        {
            logger.LogWarning("User {UserId} attempted to access project {ProjectId} applications without permission", 
                userId, request.ProjectId);
            
            throw new ForbiddenException($"You do not have access to Project with ID '{request.ProjectId}'");
        }
        
        var offset = (request.PageNo - 1) * request.PageSize;
        
        var applications = await unitOfWork.FreelancerApplicationQueriesRepository.PaginatedListAsync(
            fa => fa.ProjectId == request.ProjectId,
            offset,
            request.PageSize,
            cancellationToken);
        
        var applicationsCount = await unitOfWork.FreelancerApplicationQueriesRepository.CountAsync(
            fa => fa.ProjectId == request.ProjectId, 
            cancellationToken);

        logger.LogInformation("Retrieved {Count} applications for project {ProjectId} out of {TotalCount}", 
            applications.Count, request.ProjectId, applicationsCount);

        return new PaginatedResultModel<FreelancerApplication>
        {
            Items = applications.ToList(),
            TotalCount = applicationsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}