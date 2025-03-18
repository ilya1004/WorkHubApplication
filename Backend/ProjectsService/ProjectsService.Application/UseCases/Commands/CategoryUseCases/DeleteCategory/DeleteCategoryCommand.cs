namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest;