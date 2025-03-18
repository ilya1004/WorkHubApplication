namespace IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;

public class ChangePasswordCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null) throw new NotFoundException($"User with email '{request.Email}' not found");

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Password is not successfully changed. Errors: {errors}");
        }
    }
}