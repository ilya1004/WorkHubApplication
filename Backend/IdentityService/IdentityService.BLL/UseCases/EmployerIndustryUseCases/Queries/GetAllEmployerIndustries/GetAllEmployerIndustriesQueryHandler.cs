using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetAllEmployerIndustries;

public class GetAllEmployerIndustriesQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetAllEmployerIndustriesQueryHandler> logger) : IRequestHandler<GetAllEmployerIndustriesQuery, PaginatedResultModel<EmployerIndustry>>
{
    public async Task<PaginatedResultModel<EmployerIndustry>> Handle(GetAllEmployerIndustriesQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting paginated list of employer industries. Page: {PageNo}, Size: {PageSize}", 
            request.PageNo, request.PageSize);

        var offset = (request.PageNo - 1) * request.PageSize;

        var industries = await unitOfWork.EmployerIndustriesRepository.PaginatedListAllAsync(
            offset,
            request.PageSize,
            cancellationToken);

        var industriesCount = await unitOfWork.EmployerIndustriesRepository.CountAllAsync(cancellationToken);

        logger.LogInformation("Retrieved {Count} industries out of {TotalCount}", industries.Count, industriesCount);

        return new PaginatedResultModel<EmployerIndustry>
        {
            Items = industries.ToList(),
            TotalCount = industriesCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}