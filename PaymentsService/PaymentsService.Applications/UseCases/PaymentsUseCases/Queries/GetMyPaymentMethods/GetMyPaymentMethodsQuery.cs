namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetMyPaymentMethods;

public sealed record GetMyPaymentMethodsQuery : IRequest<IEnumerable<PaymentMethodDto>>;