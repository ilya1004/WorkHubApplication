using PaymentsService.Applications.Models;
using PaymentsService.Domain.Models;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentsQuery;

public sealed record GetEmployerPaymentsQuery(
    Guid? ProjectId,
    int PageNo,
    int PageSize) : IRequest<PaginatedResultModel<ChargeModel>>;