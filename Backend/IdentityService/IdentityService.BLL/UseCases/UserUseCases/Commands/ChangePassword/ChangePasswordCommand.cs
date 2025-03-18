namespace IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;

public sealed record ChangePasswordCommand(string Email, string CurrentPassword, string NewPassword) : IRequest;