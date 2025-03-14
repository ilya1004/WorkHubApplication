namespace PaymentsService.Applications.UseCases.PaymentMethodUseCases.Commands.DeletePaymentMethod;

public sealed record DeletePaymentMethodCommand(string PaymentMethodId) : IRequest;