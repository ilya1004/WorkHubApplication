using PaymentsService.Application.Models;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllEmployerAccounts;

public class GetAllEmployerAccountsQueryHandler(
    IEmployerAccountsService employerAccountsService,
    ILogger<GetAllEmployerAccountsQueryHandler> logger) : IRequestHandler<GetAllEmployerAccountsQuery, PaginatedResultModel<EmployerAccountModel>>
{
    public async Task<PaginatedResultModel<EmployerAccountModel>> Handle(GetAllEmployerAccountsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving employer accounts page {PageNo}, size {PageSize}", request.PageNo, request.PageSize);

        var result = await employerAccountsService.GetAllEmployerAccountsAsync(cancellationToken);

        var offset = (request.PageNo - 1) * request.PageSize;
        var resultList = result.Skip(offset).Take(request.PageSize).ToList();
        
        logger.LogInformation("Retrieved {Count} employer accounts", resultList.Count);

        return new PaginatedResultModel<EmployerAccountModel>
        {
            Items = resultList,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = resultList.Count
        };
    }
}