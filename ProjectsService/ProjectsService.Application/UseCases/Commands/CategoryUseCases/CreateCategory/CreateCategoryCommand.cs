using MediatR;

namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;

public sealed record CreateCategoryCommand(string Name) : IRequest;