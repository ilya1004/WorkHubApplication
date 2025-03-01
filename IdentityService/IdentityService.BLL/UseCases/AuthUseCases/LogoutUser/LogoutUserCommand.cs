namespace IdentityService.BLL.UseCases.AuthUseCases.LogoutUser;

public sealed record LogoutUserCommand(Guid UserId) : IRequest;