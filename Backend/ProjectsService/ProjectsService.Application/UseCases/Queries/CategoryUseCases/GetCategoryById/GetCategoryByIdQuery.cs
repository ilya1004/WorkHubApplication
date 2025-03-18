namespace ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<Category>;