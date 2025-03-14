using IdentityService.DAL.Abstractions.RedisService;

namespace IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    UserManager<AppUser> userManager,
    ICachedService cachedService) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null) throw new BadRequestException($"A user with the email '{request.Email}' not exist.");

        if (user.EmailConfirmed) throw new BadRequestException("Your email is already confirmed.");

        var token = await cachedService.GetAsync(request.Code);

        if (string.IsNullOrEmpty(token)) throw new BadRequestException("Invalid verification code.");

        var result = await userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded) throw new BadRequestException("Email confirmation token is invalid");

        await cachedService.DeleteAsync(request.Code);
    }
}