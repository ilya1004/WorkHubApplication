using PaymentsService.Application.Models;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerMyTransfers;

public class GetFreelancerMyTransfersQueryHandler(
    ITransfersService transfersService,
    IUserContext userContext,
    ILogger<GetFreelancerMyTransfersQueryHandler> logger) : IRequestHandler<GetFreelancerMyTransfersQuery, PaginatedResultModel<TransferModel>>
{
    public async Task<PaginatedResultModel<TransferModel>> Handle(GetFreelancerMyTransfersQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Retrieving transfers for freelancer {UserId}, project {ProjectId}, page {PageNo}, size {PageSize}", 
            userId, request.ProjectId, request.PageNo, request.PageSize);

        var result = await transfersService.GetFreelancerTransfersAsync(
            userId, request.ProjectId, cancellationToken);

        var offset = (request.PageNo - 1) * request.PageSize;
        var resultList = result.Skip(offset).Take(request.PageSize).ToList();
        
        logger.LogInformation("Retrieved {Count} transfers for freelancer {UserId}, project {ProjectId}", 
            resultList.Count, userId, request.ProjectId);

        return new PaginatedResultModel<TransferModel>
        {
            Items = resultList,
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = resultList.Count
        };
    }
}