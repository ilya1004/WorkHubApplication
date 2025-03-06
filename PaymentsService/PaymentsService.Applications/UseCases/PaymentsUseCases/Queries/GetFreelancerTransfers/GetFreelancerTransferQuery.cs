using PaymentsService.Applications.Models;
using PaymentsService.Domain.Models;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetFreelancerTransfers;

public sealed record GetFreelancerTransferQuery(
    Guid? ProjectId,
    int PageNo,
    int PageSize) : IRequest<PaginatedResultModel<TransferModel>>;