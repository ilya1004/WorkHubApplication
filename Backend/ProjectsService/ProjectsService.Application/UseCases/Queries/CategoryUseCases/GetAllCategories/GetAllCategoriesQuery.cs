using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetAllCategories;

public sealed record GetAllCategoriesQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<Category>>;