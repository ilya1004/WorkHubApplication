namespace ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetCategoryById;

public class GetCategoryByIdQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetCategoryByIdQuery, Category>
{
    public async Task<Category> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException($"Category with ID '{request.Id}' not found");
        }
        
        return category;
    }
}