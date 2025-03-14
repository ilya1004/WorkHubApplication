using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentMethodUseCases.Queries.GetMyPaymentMethods;

public sealed record GetMyPaymentMethodsQuery : IRequest<IEnumerable<PaymentMethodModel>>;