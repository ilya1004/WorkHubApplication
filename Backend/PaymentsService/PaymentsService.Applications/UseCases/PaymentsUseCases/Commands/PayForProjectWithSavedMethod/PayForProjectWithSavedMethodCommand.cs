namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;

public sealed record PayForProjectWithSavedMethodCommand(Guid ProjectId, string PaymentMethodId) : IRequest;