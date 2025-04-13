using PaymentsService.Application.Models;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentIntents;

public class GetEmployerPaymentIntentsQueryHandler(
    IUserContext userContext,
    ITransfersService transfersService,
    ILogger<GetEmployerMyPaymentsQueryHandler> logger) : IRequestHandler<GetEmployerPaymentIntentsQuery, PaginatedResultModel<PaymentIntentModel>>
{
    public async Task<PaginatedResultModel<PaymentIntentModel>> Handle(GetEmployerPaymentIntentsQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Retrieving payment intents for employer {UserId}, project {ProjectId}, page {PageNo}, size {PageSize}", 
            userId, request.ProjectId, request.PageNo, request.PageSize);

        var result = await transfersService.GetEmployerPaymentIntentsAsync(
            userId, request.ProjectId, cancellationToken);

        var offset = (request.PageNo - 1) * request.PageSize;
        var resultList = result.Skip(offset).Take(request.PageSize).ToList();
        
        logger.LogInformation("Retrieved {Count} payment intents for employer {UserId}, project {ProjectId}", 
            resultList.Count, userId, request.ProjectId);

        return new PaginatedResultModel<PaymentIntentModel>
        {
            Items = resultList,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = resultList.Count
        };
    }
}