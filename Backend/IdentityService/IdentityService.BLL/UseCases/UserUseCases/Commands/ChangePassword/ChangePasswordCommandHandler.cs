namespace IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    UserManager<AppUser> userManager,
    ILogger<ChangePasswordCommandHandler> logger) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Changing password for user with email: {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found", request.Email);
            
            throw new NotFoundException($"User with email '{request.Email}' not found");
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            
            logger.LogWarning("Failed to change password for user {UserId}: {Errors}", user.Id, errors);
            
            throw new BadRequestException($"Password is not successfully changed. Errors: {errors}");
        }

        logger.LogInformation("Successfully changed password for user {UserId}", user.Id);
    }
}