namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.SavePaymentMethod;

public sealed record SavePaymentMethodCommand(Guid PaymentMethodId) : IRequest;