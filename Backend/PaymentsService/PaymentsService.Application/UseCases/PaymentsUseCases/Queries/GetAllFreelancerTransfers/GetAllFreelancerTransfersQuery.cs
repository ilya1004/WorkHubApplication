using PaymentsService.Application.Models;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllFreelancerTransfers;

public sealed record GetAllFreelancerTransfersQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<TransferModel>>;