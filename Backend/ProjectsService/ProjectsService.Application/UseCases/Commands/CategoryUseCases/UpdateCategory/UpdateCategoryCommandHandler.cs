namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.UpdateCategory;

public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateCategoryCommand>
{
    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException($"Category with ID '{request.Id}' not found");
        }
        
        mapper.Map(request, category);
        
        await unitOfWork.CategoryCommandsRepository.UpdateAsync(category, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}