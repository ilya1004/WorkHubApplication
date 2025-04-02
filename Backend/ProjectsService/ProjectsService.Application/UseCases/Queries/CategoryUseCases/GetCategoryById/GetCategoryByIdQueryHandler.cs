namespace ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetCategoryById;

public class GetCategoryByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetCategoryByIdQueryHandler> logger) : IRequestHandler<GetCategoryByIdQuery, Category>
{
    public async Task<Category> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting category by ID: {CategoryId}", request.Id);

        var category = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            logger.LogWarning("Category with ID {CategoryId} not found", request.Id);
            
            throw new NotFoundException($"Category with ID '{request.Id}' not found");
        }
        
        logger.LogInformation("Successfully retrieved category with ID: {CategoryId}", request.Id);
        
        return category;
    }
}
