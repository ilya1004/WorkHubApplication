namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.DeleteCategory;

public class DeleteCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteCategoryCommandHandler> logger) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting deletion of category with ID: {CategoryId}", request.Id);

        var category = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            logger.LogWarning("Category with ID '{CategoryId}' not found", request.Id);
            throw new NotFoundException($"Category with ID '{request.Id}' not found");
        }
        
        logger.LogInformation("Deleting category with ID: {CategoryId}", request.Id);
        
        await unitOfWork.CategoryCommandsRepository.DeleteAsync(category, cancellationToken);

        logger.LogInformation("Saving changes to database");
        
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully deleted category with ID: {CategoryId}", request.Id);
    }
}