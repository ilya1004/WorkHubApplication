using IdentityService.BLL.Services.EmailSender;
using System.Net;

namespace IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;

public class ForgotPasswordCommandHandler(
    UserManager<AppUser> userManager,
    IEmailSender emailSender) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null) throw new NotFoundException("User with this email does not exist.");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var resetUrl = $"{request.ResetUrl}?email={user.Email}&token={WebUtility.UrlEncode(token)}";

        await emailSender.SendPasswordReset(user.Email!, resetUrl, cancellationToken);
    }
}