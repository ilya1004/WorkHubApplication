using FluentEmail.Core;
using IdentityService.BLL.Abstractions.EmailSender;

namespace IdentityService.BLL.Services.EmailSender;

public class EmailSender(IFluentEmail fluentEmail) : IEmailSender
{
    public async Task SendEmailConfirmation(string userEmail, string code, CancellationToken cancellationToken)
    {
        await fluentEmail
            .To(userEmail)
            .Subject("Email verification from WorkHubApplication")
            .Body($"Your verification code is <div>{code}</div>", true)
            .SendAsync(cancellationToken);
    }

    public async Task SendPasswordReset(string userEmail, string resetUrl, CancellationToken cancellationToken)
    {
        await fluentEmail
            .To(userEmail)
            .Subject("Reset password from WorkHubApplication")
            .Body($"Click the link to reset your password: <a href='{resetUrl}'>Reset!</a>", true)
            .SendAsync(cancellationToken);
    }
}