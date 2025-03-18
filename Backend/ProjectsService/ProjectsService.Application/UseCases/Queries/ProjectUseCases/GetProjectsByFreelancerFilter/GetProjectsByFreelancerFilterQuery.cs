using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;

public sealed record GetProjectsByFreelancerFilterQuery(
    ProjectStatus? ProjectStatus,
    Guid? EmployerId,
    int PageNo = 1,
    int PageSize = 10) : IRequest<PaginatedResultModel<Project>>;