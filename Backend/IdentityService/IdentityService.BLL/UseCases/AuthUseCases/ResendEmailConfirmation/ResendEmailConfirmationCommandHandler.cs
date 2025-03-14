using IdentityService.BLL.Services.EmailSender;
using IdentityService.DAL.Abstractions.RedisService;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;

public class ResendEmailConfirmationCommandHandler(
    UserManager<AppUser> userManager, 
    IEmailSender emailSender,
    ICachedService cachedService,
    IConfiguration configuration) : IRequestHandler<ResendEmailConfirmationCommand>
{
    public async Task Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null) throw new BadRequestException($"A user with the email '{request.Email}' not exist.");

        if (user.EmailConfirmed) throw new BadRequestException($"Your email is already confirmed.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        string code;
        do
        {
            code = new Random().Next(100000, 999999).ToString();
        } while (await cachedService.ExistsAsync(code));

        await cachedService.SetAsync(code, token, TimeSpan.FromHours(
            int.Parse(configuration.GetRequiredSection("IdentityTokenExpirationTimeInHours").Value!)));

        await emailSender.SendEmailConfirmation(user.Email!, code, cancellationToken);   
    }
}