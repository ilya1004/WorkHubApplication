using FluentEmail.Core;
using IdentityService.BLL.Abstractions.EmailSender;

namespace IdentityService.BLL.Services.EmailSender;

public class EmailSender : IEmailSender
{
    private readonly IFluentEmail _fluentEmail;

    public EmailSender(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task SendEmailConfirmation(string userEmail, string token, CancellationToken cancellationToken)
    {
        await _fluentEmail
            .To(userEmail)
            .Subject("Email verification from WorkHubApplication")
            .Body($"Your verification code is <div>{token}</div>", true)
            .SendAsync(cancellationToken);
    }

    public async Task SendPasswordReset(string userEmail, string resetUrl, CancellationToken cancellationToken)
    {
        await _fluentEmail
            .To(userEmail)
            .Subject("Reset password from WorkHubApplication")
            .Body($"Click the link to reset your password: <a href='{resetUrl}'>Reset</a>", true)
            .SendAsync(cancellationToken);
    }
}