
namespace IdentityService.BLL.UseCases.AuthUseCases.LogoutUser;

public class LogoutUserCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await userManager.UpdateAsync(user);
    }
}
