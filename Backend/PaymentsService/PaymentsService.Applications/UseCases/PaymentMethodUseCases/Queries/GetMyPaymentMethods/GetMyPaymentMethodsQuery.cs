using PaymentsService.Domain.Models;

namespace PaymentsService.Applications.UseCases.PaymentMethodUseCases.Queries.GetMyPaymentMethods;

public sealed record GetMyPaymentMethodsQuery : IRequest<IEnumerable<PaymentMethodModel>>;