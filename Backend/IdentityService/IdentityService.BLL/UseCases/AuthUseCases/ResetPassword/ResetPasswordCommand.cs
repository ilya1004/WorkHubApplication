namespace IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string NewPassword, string Code) : IRequest;