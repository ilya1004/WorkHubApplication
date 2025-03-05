using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.CreateSetupIntent;

public class CreateSetupIntentCommandHandler(
    IEmployerPaymentsService employerPaymentsService,
    IUserContext userContext) : IRequestHandler<CreateSetupIntentCommand, string>
{
    public async Task<string> Handle(CreateSetupIntentCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        var clientSecret = await employerPaymentsService.CreateSetupIntent(userId, cancellationToken);

        return clientSecret;
    }
}