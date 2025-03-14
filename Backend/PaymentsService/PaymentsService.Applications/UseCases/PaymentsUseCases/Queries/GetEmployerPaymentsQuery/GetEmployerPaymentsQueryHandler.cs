using PaymentsService.Applications.Models;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentsQuery;

public class GetEmployerPaymentsQueryHandler(
    IUserContext userContext,
    ITransfersService transfersService) : IRequestHandler<GetEmployerPaymentsQuery, PaginatedResultModel<ChargeModel>>
{
    public async Task<PaginatedResultModel<ChargeModel>> Handle(GetEmployerPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();

        var result = await transfersService.GetEmployerPaymentsAsync(
            userId, request.ProjectId, cancellationToken);

        var offset = (request.PageNo - 1) * request.PageSize;
        var resultList = result.Skip(offset).Take(request.PageSize).ToList();

        return new PaginatedResultModel<ChargeModel>
        {
            Items = resultList,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = resultList.Count
        };
    }
}