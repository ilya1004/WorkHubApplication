using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetAllCategories;

public class GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllCategoriesQuery, PaginatedResultModel<Category>>
{
    public async Task<PaginatedResultModel<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var categories = await unitOfWork.CategoryQueriesRepository.PaginatedListAllAsync(
            offset,
            request.PageSize,
            cancellationToken);
        
        var categoriesCount = await unitOfWork.CategoryQueriesRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<Category>
        {
            Items = categories.ToList(),
            TotalCount = categoriesCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}