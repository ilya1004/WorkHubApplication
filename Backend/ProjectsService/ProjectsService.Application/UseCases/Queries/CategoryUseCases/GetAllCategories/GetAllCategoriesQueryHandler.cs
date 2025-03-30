using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetAllCategories;

public class GetAllCategoriesQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetAllCategoriesQueryHandler> logger) : IRequestHandler<GetAllCategoriesQuery, PaginatedResultModel<Category>>
{
    public async Task<PaginatedResultModel<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all categories with pagination - Page: {PageNo}, Size: {PageSize}", 
            request.PageNo, request.PageSize);

        var offset = (request.PageNo - 1) * request.PageSize;

        var categories = await unitOfWork.CategoryQueriesRepository.PaginatedListAllAsync(
            offset,
            request.PageSize,
            cancellationToken);
        
        var categoriesCount = await unitOfWork.CategoryQueriesRepository.CountAllAsync(cancellationToken);

        logger.LogInformation("Retrieved {Count} categories out of {TotalCount}", 
            categories.Count, categoriesCount);

        return new PaginatedResultModel<Category>
        {
            Items = categories.ToList(),
            TotalCount = categoriesCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}