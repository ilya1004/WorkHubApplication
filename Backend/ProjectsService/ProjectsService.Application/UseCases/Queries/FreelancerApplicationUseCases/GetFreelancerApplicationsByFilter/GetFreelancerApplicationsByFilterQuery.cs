using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;

public sealed record GetFreelancerApplicationsByFilterQuery(
    DateTime? StartDate,
    DateTime? EndDate,
    ApplicationStatus? ApplicationStatus,
    int PageNo,
    int PageSize) : IRequest<PaginatedResultModel<FreelancerApplication>>; 