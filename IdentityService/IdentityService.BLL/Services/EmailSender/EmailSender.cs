using FluentEmail.Core;

namespace IdentityService.BLL.Services.EmailSender;

public class EmailSender : IEmailSender
{
    private readonly IFluentEmail _fluentEmail;
    public EmailSender(IFluentEmail fluentEmail)
    {       
        _fluentEmail = fluentEmail;
    }

    public async Task SendEmailConfirmation(string userEmail, string code, CancellationToken cancellationToken)
    {
        await _fluentEmail
            .To(userEmail)
            .Subject("Email verification from WorkHubApplication")
            .Body($"Your verification code is <div>{code}</div>", true)
            .SendAsync(cancellationToken);
    }
}
