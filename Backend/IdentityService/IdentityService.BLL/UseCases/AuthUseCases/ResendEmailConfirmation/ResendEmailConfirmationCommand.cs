namespace IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;

public sealed record ResendEmailConfirmationCommand(string Email) : IRequest;