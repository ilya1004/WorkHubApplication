using MediatR;
using ProjectsService.Domain.Abstractions.Data;

namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;

public class CreateCategoryCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand>
{
    public Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = unitOfWork
    }
}