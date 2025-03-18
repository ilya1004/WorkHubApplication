namespace PaymentsService.Application.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;

public sealed record PayForProjectWithSavedMethodCommand(Guid ProjectId, string PaymentMethodId) : IRequest;