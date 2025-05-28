using IdentityService.DAL.Abstractions.RedisService;

namespace IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    UserManager<AppUser> userManager,
    ICachedService cachedService,
    ILogger<ConfirmEmailCommandHandler> logger) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting email confirmation for {Email}", request.Email);

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
        
        logger.LogInformation("Retrieving verification code {Code} from cache", request.Code);
        
        var token = await cachedService.GetAsync(request.Code);

        if (string.IsNullOrEmpty(token))
        {
            logger.LogWarning("Invalid verification code {Code}", request.Code);
            
            throw new BadRequestException("Invalid verification code.");
        }

        logger.LogInformation("Confirming email for user {UserId}", user.Id);
        
        var result = await userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            logger.LogWarning("Invalid email confirmation token for user {UserId}", user.Id);
            
            throw new BadRequestException("Email confirmation token is invalid");
        }

        await cachedService.DeleteAsync(request.Code);
        
        logger.LogInformation("Email confirmed successfully for user {UserId}", user.Id);
    }
}