using PaymentsService.Application.Models;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllFreelancerAccounts;

public class GetAllFreelancerAccountsQueryHandler(
    IFreelancerAccountsService freelancerAccountsService,
    ILogger<GetAllFreelancerAccountsQueryHandler> logger) : IRequestHandler<GetAllFreelancerAccountsQuery, PaginatedResultModel<FreelancerAccountModel>>
{
    public async Task<PaginatedResultModel<FreelancerAccountModel>> Handle(GetAllFreelancerAccountsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving freelancer accounts page {PageNo}, size {PageSize}", request.PageNo, request.PageSize);

        var result = await freelancerAccountsService.GetAllFreelancerAccountsAsync(cancellationToken);

        var offset = (request.PageNo - 1) * request.PageSize;
        var resultList = result.Skip(offset).Take(request.PageSize).ToList();
        
        logger.LogInformation("Retrieved {Count} freelancer accounts", resultList.Count);

        return new PaginatedResultModel<FreelancerAccountModel>
        {
            Items = resultList,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = resultList.Count
        };
    }
}