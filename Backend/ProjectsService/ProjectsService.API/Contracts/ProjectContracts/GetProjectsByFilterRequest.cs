using ProjectsService.Domain.Enums;

namespace ProjectsService.API.Contracts.ProjectContracts;

public sealed record GetProjectsByFilterRequest(
    string? Title,
    decimal? BudgetFrom, 
    decimal? BudgetTo,
    Guid? CategoryId,
    Guid? EmployerId,
    ProjectStatus? ProjectStatus,
    int PageNo = 1,
    int PageSize = 10);