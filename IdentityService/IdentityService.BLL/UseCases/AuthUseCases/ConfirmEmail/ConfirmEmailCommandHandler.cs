
namespace IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;

public class ConfirmEmailCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new BadRequestException($"A user with the email '{request.Email}' not exist.");
        }

        var result = await userManager.ConfirmEmailAsync(user, request.Code);

        if (!result.Succeeded)
        {
            throw new BadRequestException("Email confirmation token is invalid");
        }
    }
}
