using IdentityService.BLL.Services.EmailSender;

namespace IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;

public class ResendEmailConfirmationCommandHandler(UserManager<AppUser> userManager, IEmailSender emailSender) : IRequestHandler<ResendEmailConfirmationCommand>
{
    public async Task Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new BadRequestException($"A user with the email '{request.Email}' not exist.");
        }

        if (user.EmailConfirmed)
        {
            throw new BadRequestException($"Your email is already confirmed.");
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        await emailSender.SendEmailConfirmation(user.Email!, token, cancellationToken);

    }
}
