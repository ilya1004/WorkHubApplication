namespace PaymentsService.Application.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;

public sealed record ConfirmPaymentForProjectCommand(Guid ProjectId) : IRequest;