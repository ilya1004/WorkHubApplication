using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.DAL.Abstractions.RedisService;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;

public class ResendEmailConfirmationCommandHandler(
    UserManager<AppUser> userManager, 
    IEmailSender emailSender,
    ICachedService cachedService,
    IConfiguration configuration,
    ILogger<ResendEmailConfirmationCommandHandler> logger) : IRequestHandler<ResendEmailConfirmationCommand>
{
    public async Task Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Resending email confirmation to {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found", request.Email);
            
            throw new BadRequestException($"A user with the email '{request.Email}' not exist.");
        }

        if (user.EmailConfirmed)
        {
            logger.LogInformation("Email {Email} already confirmed", request.Email);
            
            throw new BadRequestException("Your email is already confirmed.");
        }

        logger.LogInformation("Generating new confirmation token for user {UserId}", user.Id);
        
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        string code;
        var random = new Random();
        do
        {
            code = random.Next(100000, 999999).ToString();
        }
        while (await cachedService.ExistsAsync(code, cancellationToken));

        logger.LogInformation("Storing confirmation code {Code} in cache", code);
        
        await cachedService.SetAsync(code, token, TimeSpan.FromHours(
            int.Parse(configuration.GetRequiredSection("IdentityTokenExpirationTimeInHours").Value!)), cancellationToken);

        logger.LogInformation("Sending confirmation email to {Email}", user.Email);
        
        await emailSender.SendEmailConfirmation(user.Email!, code, cancellationToken);
        
        logger.LogInformation("Confirmation email sent to {Email}", user.Email);
    }
}