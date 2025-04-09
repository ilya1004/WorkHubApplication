using IdentityService.BLL.Abstractions.UserContext;

namespace IdentityService.BLL.UseCases.AuthUseCases.LogoutUser;

public class LogoutUserCommandHandler(
    UserManager<AppUser> userManager,
    IUserContext userContext,
    ILogger<LogoutUserCommandHandler> logger) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Logout requested for user with ID '{UserId}'", userId);

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            logger.LogWarning("User with ID '{UserId}' not found during logout", userId);
            
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        if (user.RefreshToken is null && user.RefreshTokenExpiryTime is null)
        {
            logger.LogInformation("User with ID '{UserId}' has already logout", userId);
            
            return;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await userManager.UpdateAsync(user);
        
        logger.LogInformation("User with ID '{UserId}' logged out successfully", userId);
    }
}