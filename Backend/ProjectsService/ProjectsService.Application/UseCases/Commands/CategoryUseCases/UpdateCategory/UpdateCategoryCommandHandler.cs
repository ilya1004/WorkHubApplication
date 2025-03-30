namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.UpdateCategory;

public class UpdateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<UpdateCategoryCommandHandler> logger) : IRequestHandler<UpdateCategoryCommand>
{
    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting update of category with ID: {CategoryId}", request.Id);

        var category = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            logger.LogWarning("Category with ID '{CategoryId}' not found", request.Id);
            
            throw new NotFoundException($"Category with ID '{request.Id}' not found");
        }
        
        mapper.Map(request, category);
        
        logger.LogInformation("Updating category with ID: {CategoryId}", request.Id);
        
        await unitOfWork.CategoryCommandsRepository.UpdateAsync(category, cancellationToken);
        
        logger.LogInformation("Saving changes to database");
        
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully updated category with ID: {CategoryId}", request.Id);
    }
}