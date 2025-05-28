using PaymentsService.Application.Models;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentIntents;

public sealed record GetEmployerPaymentIntentsQuery(
    Guid? ProjectId,
    int PageNo,
    int PageSize) : IRequest<PaginatedResultModel<PaymentIntentModel>>;