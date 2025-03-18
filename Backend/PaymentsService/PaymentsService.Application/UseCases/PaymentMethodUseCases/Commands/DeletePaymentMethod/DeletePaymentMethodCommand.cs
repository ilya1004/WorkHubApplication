namespace PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.DeletePaymentMethod;

public sealed record DeletePaymentMethodCommand(string PaymentMethodId) : IRequest;