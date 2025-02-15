namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;

public class CreateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateCategoryCommand>
{
    public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.CategoryQueriesRepository.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

        if (category is not null)
        {
            throw new BadRequestException($"Category with name '{request.Name}' already exists.");
        }
        
        var newCategory = mapper.Map<Category>(request);
        
        await unitOfWork.CategoryCommandsRepository.AddAsync(newCategory, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}