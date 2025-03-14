namespace PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.SavePaymentMethod;

public sealed record SavePaymentMethodCommand(string PaymentMethodId) : IRequest;