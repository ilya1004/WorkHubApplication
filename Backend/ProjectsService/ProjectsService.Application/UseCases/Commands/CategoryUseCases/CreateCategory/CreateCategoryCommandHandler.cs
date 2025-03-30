namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;

public class CreateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<CreateCategoryCommandHandler> logger) : IRequestHandler<CreateCategoryCommand>
{
    public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting creation of category with name: {CategoryName}", request.Name);

        var category = await unitOfWork.CategoryQueriesRepository.FirstOrDefaultAsync(
            c => c.Name == request.Name, 
            cancellationToken);

        if (category is not null)
        {
            logger.LogWarning("Category with name '{CategoryName}' already exists", request.Name);
            
            throw new AlreadyExistsException($"Category with name '{request.Name}' already exists.");
        }
        
        var newCategory = mapper.Map<Category>(request);
        
        logger.LogInformation("Adding new category with ID: {CategoryId}", newCategory.Id);
        
        await unitOfWork.CategoryCommandsRepository.AddAsync(newCategory, cancellationToken);
        
        logger.LogInformation("Saving changes to database");
        
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully created category with ID: {CategoryId}", newCategory.Id);
    }
}
