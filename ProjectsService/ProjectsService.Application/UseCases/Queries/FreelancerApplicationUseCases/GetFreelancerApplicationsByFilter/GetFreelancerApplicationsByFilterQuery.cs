using ProjectsService.Application.Models;
using ProjectsService.Domain.Enums;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;

public sealed record GetFreelancerApplicationsByFilterQuery(
    Guid FreelancerId,
    DateTime? StartDate,
    DateTime? EndDate,
    ApplicationStatus? ApplicationStatus,
    int PageNo,
    int PageSize)
    : IRequest<PaginatedResultModel<FreelancerApplication>>; 