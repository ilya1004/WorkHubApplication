using PaymentsService.Application.Models;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;

public sealed record GetEmployerMyPaymentsQuery(
    Guid? ProjectId,
    int PageNo,
    int PageSize) : IRequest<PaginatedResultModel<ChargeModel>>;