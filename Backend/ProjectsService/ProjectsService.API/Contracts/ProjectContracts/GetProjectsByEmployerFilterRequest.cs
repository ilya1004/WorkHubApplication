using ProjectsService.Domain.Enums;

namespace ProjectsService.API.Contracts.ProjectContracts;

public sealed record GetProjectsByEmployerFilterRequest(
    DateTime? UpdatedAtStartDate,
    DateTime? UpdatedAtEndDate,
    ProjectStatus? ProjectStatus,
    bool? AcceptanceRequestedAndNotConfirmed,
    int PageNo = 1,
    int PageSize = 10);