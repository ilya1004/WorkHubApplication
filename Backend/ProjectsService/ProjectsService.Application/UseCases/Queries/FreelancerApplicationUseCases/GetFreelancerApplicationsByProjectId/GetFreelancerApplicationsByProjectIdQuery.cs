using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByProjectId;

public sealed record GetFreelancerApplicationsByProjectIdQuery(Guid ProjectId, int PageNo, int PageSize) 
    : IRequest<PaginatedResultModel<FreelancerApplication>>;