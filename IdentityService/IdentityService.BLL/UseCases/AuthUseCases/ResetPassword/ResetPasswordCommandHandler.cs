
namespace IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;

public class ResetPasswordCommandHandler(
    UserManager<AppUser> userManager) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new NotFoundException("User with this email does not exist.");
        }

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Password is not successfully changed. Errors: {errors}");
        }
    }
}
