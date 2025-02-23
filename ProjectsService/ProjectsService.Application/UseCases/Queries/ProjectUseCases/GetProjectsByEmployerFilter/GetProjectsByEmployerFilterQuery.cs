using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;

public sealed record GetProjectsByEmployerFilterQuery(
    DateTime? UpdatedAtStartDate,
    DateTime? UpdatedAtEndDate,
    ProjectStatus? ProjectStatus,
    bool? AcceptanceRequestedAndNotConfirmed,
    int PageNo = 1,
    int PageSize = 10) : IRequest<PaginatedResultModel<Project>>;