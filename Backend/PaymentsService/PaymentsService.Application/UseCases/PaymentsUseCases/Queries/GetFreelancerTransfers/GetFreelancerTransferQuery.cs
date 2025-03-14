using PaymentsService.Application.Models;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerTransfers;

public sealed record GetFreelancerTransferQuery(
    Guid? ProjectId,
    int PageNo,
    int PageSize) : IRequest<PaginatedResultModel<TransferModel>>;