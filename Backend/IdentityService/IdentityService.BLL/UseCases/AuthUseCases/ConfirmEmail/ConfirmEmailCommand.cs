namespace IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Code) : IRequest;