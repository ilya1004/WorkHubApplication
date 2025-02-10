using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetAllEmployerIndustries;

public class GetAllEmployerIndustriesQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetAllEmployerIndustriesQuery, PaginatedResultModel<EmployerIndustry>>
{
    public async Task<PaginatedResultModel<EmployerIndustry>> Handle(GetAllEmployerIndustriesQuery request, CancellationToken cancellationToken)
    {
        int offset = (request.PageNo - 1) * request.PageSize;

        var industries = await unitOfWork.EmployerIndustriesRepository.PaginatedListAllAsync(
            offset,
            request.PageSize,
            cancellationToken);

        var industriesCount = await unitOfWork.EmployerIndustriesRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<EmployerIndustry>
        {
            Items = industries.ToList(),
            TotalCount = industriesCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}
