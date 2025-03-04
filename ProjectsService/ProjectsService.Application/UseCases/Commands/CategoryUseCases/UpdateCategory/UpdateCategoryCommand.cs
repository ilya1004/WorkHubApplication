namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid Id, string Name) : IRequest;