using PaymentsService.Application.Models;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllEmployerPayments;

public class GetAllEmployerPaymentsQueryHandler(
    ITransfersService transfersService,
    ILogger<GetEmployerMyPaymentsQueryHandler> logger) : IRequestHandler<GetAllEmployerPaymentsQuery, PaginatedResultModel<ChargeModel>>
{
    public async Task<PaginatedResultModel<ChargeModel>> Handle(GetAllEmployerPaymentsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving payments by page {PageNo}, size {PageSize}", request.PageNo, request.PageSize);

        var result = await transfersService.GetAllEmployerPaymentsAsync(cancellationToken);

        var offset = (request.PageNo - 1) * request.PageSize;
        var resultList = result.Skip(offset).Take(request.PageSize).ToList();
        
        logger.LogInformation("Retrieved {Count} payments", resultList.Count);

        return new PaginatedResultModel<ChargeModel>
        {
            Items = resultList,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = resultList.Count
        };
    }
}