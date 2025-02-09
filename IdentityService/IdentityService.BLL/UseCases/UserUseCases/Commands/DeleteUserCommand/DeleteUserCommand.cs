namespace IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;

public sealed record DeleteUserCommand(Guid UserId) : IRequest;