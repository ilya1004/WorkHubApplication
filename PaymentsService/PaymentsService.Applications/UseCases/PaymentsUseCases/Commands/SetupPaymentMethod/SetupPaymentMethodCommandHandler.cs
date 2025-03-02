using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.SetupPaymentMethod;

public class SetupPaymentMethodCommandHandler(
    IEmployerPaymentsService employerPaymentsService,
    IUserContext userContext) : IRequestHandler<SetupPaymentMethodCommand, string>
{
    public async Task<string> Handle(SetupPaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        var clientSecret = await employerPaymentsService.SetupPaymentMethodAsync(userId, cancellationToken);

        return clientSecret;
    }
}