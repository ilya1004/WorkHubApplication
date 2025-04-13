using PaymentsService.Application.Models;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllEmployerPayments;

public sealed record GetAllEmployerPaymentsQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<ChargeModel>>;