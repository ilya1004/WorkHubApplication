using PaymentsService.Applications.Models;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetFreelancerTransfers;

public class GetFreelancerTransferQueryHandler(
    ITransfersService transfersService,
    IUserContext userContext) : IRequestHandler<GetFreelancerTransferQuery, PaginatedResultModel<TransferModel>>
{
    public async Task<PaginatedResultModel<TransferModel>> Handle(GetFreelancerTransferQuery request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();

        var result = await transfersService.GetFreelancerTransfersAsync(
            userId, request.ProjectId, cancellationToken);

        var offset = (request.PageNo - 1) * request.PageSize;
        var resultList = result.Skip(offset).Take(request.PageSize).ToList();

        return new PaginatedResultModel<TransferModel>
        {
            Items = resultList,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = resultList.Count
        };
    }
}