using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.DAL.Abstractions.RedisService;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;

public class ForgotPasswordCommandHandler(
    UserManager<AppUser> userManager,
    IEmailSender emailSender,
    ICachedService cachedService,
    IConfiguration configuration,
    ILogger<ForgotPasswordCommandHandler> logger) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing password reset request for {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found", request.Email);
            
            throw new NotFoundException("User with this email does not exist.");
        }

        logger.LogInformation("Generating password reset token for user {UserId}", user.Id);
        
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        string code;
        var random = new Random();
        do
        {
            code = random.Next(100000, 999999).ToString();
        } 
        while (await cachedService.ExistsAsync(code, cancellationToken));

        logger.LogInformation("Storing reset code {Code} in cache", code);
        
        await cachedService.SetAsync(code, token, TimeSpan.FromHours(
            int.Parse(configuration.GetRequiredSection("IdentityTokenExpirationTimeInHours").Value!)), cancellationToken);

        var resetUrl = $"{request.ResetUrl}?email={user.Email}&code={code}";
        
        logger.LogInformation("Sending password reset email to {Email}", user.Email);
        
        await emailSender.SendPasswordReset(user.Email!, resetUrl, cancellationToken);
        
        logger.LogInformation("Password reset email sent to {Email}", user.Email);
    }
}