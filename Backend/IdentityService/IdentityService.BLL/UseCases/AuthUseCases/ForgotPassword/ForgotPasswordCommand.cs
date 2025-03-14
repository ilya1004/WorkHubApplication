namespace IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email, string ResetUrl) : IRequest;