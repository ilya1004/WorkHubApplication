namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.DeleteCategory;

public class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException($"Category with ID '{request.Id}' not found");
        }
        
        await unitOfWork.CategoryCommandsRepository.DeleteAsync(category, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}