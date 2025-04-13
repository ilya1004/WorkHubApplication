using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetMyFreelancerApplicationsByFilter;

public sealed record GetMyFreelancerApplicationsByFilterQuery(
    DateTime? StartDate,
    DateTime? EndDate,
    ApplicationStatus? ApplicationStatus,
    int PageNo,
    int PageSize) : IRequest<PaginatedResultModel<FreelancerApplication>>; 