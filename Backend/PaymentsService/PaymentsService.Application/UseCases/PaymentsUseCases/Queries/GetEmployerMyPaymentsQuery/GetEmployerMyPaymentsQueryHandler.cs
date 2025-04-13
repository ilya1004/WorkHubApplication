using PaymentsService.Application.Models;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;

public class GetEmployerMyPaymentsQueryHandler(
    IUserContext userContext,
    ITransfersService transfersService,
    ILogger<GetEmployerMyPaymentsQueryHandler> logger) : IRequestHandler<GetEmployerMyPaymentsQuery, PaginatedResultModel<ChargeModel>>
{
    public async Task<PaginatedResultModel<ChargeModel>> Handle(GetEmployerMyPaymentsQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Retrieving payments for employer {UserId}, project {ProjectId}, page {PageNo}, size {PageSize}", 
            userId, request.ProjectId, request.PageNo, request.PageSize);

        var result = await transfersService.GetEmployerPaymentsAsync(
            userId, request.ProjectId, cancellationToken);

        var offset = (request.PageNo - 1) * request.PageSize;
        var resultList = result.Skip(offset).Take(request.PageSize).ToList();
        
        logger.LogInformation("Retrieved {Count} payments for employer {UserId}, project {ProjectId}", 
            resultList.Count, userId, request.ProjectId);

        return new PaginatedResultModel<ChargeModel>
        {
            Items = resultList,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = resultList.Count
        };
    }
}
