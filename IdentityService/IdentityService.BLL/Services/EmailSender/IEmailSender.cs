namespace IdentityService.BLL.Services.EmailSender;

public interface IEmailSender
{
    Task SendEmailConfirmation(string userEmail, string code, CancellationToken cancellationToken);
}
