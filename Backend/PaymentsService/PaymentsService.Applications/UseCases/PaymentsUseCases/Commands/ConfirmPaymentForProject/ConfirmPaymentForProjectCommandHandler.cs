using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;

public class ConfirmPaymentForProjectCommandHandler(
    IEmployerPaymentsService employerPaymentsService,
    IUserContext userContext) : IRequestHandler<ConfirmPaymentForProjectCommand>
{
    public async Task Handle(ConfirmPaymentForProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();

        await employerPaymentsService.ConfirmPaymentForProjectAsync(request.ProjectId, userId, cancellationToken);
    }
}