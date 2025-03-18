using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFilter;

public sealed record GetProjectsByFilterQuery(
    string? Title, 
    decimal? BudgetFrom, 
    decimal? BudgetTo,
    Guid? CategoryId, 
    Guid? EmployerId,
    ProjectStatus? ProjectStatus,
    int PageNo = 1, 
    int PageSize = 10) : IRequest<PaginatedResultModel<Project>>;