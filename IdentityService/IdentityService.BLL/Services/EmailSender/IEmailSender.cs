namespace IdentityService.BLL.Services.EmailSender;

public interface IEmailSender
{
    Task SendEmailConfirmation(string userEmail, string code, CancellationToken cancellationToken);
    Task SendPasswordReset(string userEmail, string resetUrl, CancellationToken cancellationToken);
}
