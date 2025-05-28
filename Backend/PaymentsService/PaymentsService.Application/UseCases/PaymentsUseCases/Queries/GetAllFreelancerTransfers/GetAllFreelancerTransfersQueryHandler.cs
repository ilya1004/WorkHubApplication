using PaymentsService.Application.Models;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerMyTransfers;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllFreelancerTransfers;

public class GetAllFreelancerTransfersQueryHandler(
    ITransfersService transfersService,
    ILogger<GetFreelancerMyTransfersQueryHandler> logger) : IRequestHandler<GetAllFreelancerTransfersQuery, PaginatedResultModel<TransferModel>>
{
    public async Task<PaginatedResultModel<TransferModel>> Handle(GetAllFreelancerTransfersQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving transfers by page {PageNo}, size {PageSize}", request.PageNo, request.PageSize);

        var result = await transfersService.GetAllFreelancerTransfersAsync(cancellationToken);

        var offset = (request.PageNo - 1) * request.PageSize;
        var resultList = result.Skip(offset).Take(request.PageSize).ToList();
        
        logger.LogInformation("Retrieved {Count} transfers", resultList.Count);

        return new PaginatedResultModel<TransferModel>
        {
            Items = resultList,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = resultList.Count
        };
    }
}