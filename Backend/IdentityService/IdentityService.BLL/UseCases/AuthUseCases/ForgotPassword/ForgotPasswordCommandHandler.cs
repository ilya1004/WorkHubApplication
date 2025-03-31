using IdentityService.BLL.Services.EmailSender;
using System.Net;
using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.DAL.Abstractions.RedisService;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;

public class ForgotPasswordCommandHandler(
    UserManager<AppUser> userManager,
    IEmailSender emailSender,
    ICachedService cachedService,
    IConfiguration configuration) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null) throw new NotFoundException("User with this email does not exist.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        string code;
        var random = new Random();
        do
        {
            code = random.Next(100000, 999999).ToString();
        } 
        while (await cachedService.ExistsAsync(code));

        await cachedService.SetAsync(code, token, TimeSpan.FromHours(
            int.Parse(configuration.GetRequiredSection("IdentityTokenExpirationTimeInHours").Value!)));

        var resetUrl = $"{request.ResetUrl}?email={user.Email}&token={token}";

        await emailSender.SendPasswordReset(user.Email!, resetUrl, cancellationToken);
    }
}