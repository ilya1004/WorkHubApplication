using IdentityService.BLL.Abstractions.UserContext;

namespace IdentityService.BLL.UseCases.AuthUseCases.LogoutUser;

public class LogoutUserCommandHandler(
    UserManager<AppUser> userManager,
    IUserContext userContext) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null) throw new NotFoundException($"User with ID '{userId}' not found");

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await userManager.UpdateAsync(user);
    }
}