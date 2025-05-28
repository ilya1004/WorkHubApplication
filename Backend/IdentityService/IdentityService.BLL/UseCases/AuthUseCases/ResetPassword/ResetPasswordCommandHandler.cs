using IdentityService.DAL.Abstractions.RedisService;

namespace IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;

public class ResetPasswordCommandHandler(
    UserManager<AppUser> userManager,
    ICachedService cachedService,
    ILogger<ResetPasswordCommandHandler> logger) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing password reset for {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found", request.Email);
            
            throw new NotFoundException("User with this email does not exist.");
        }

        logger.LogInformation("Retrieving reset code {Code} from cache", request.Code);
        
        var token = await cachedService.GetAsync(request.Code, cancellationToken);
        
        if (string.IsNullOrEmpty(token))
        {
            logger.LogWarning("Invalid reset code {Code}", request.Code);
            
            throw new BadRequestException("Invalid resetting password code.");
        }
        
        logger.LogInformation("Resetting password for user {UserId}", user.Id);
        
        var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            
            logger.LogWarning("Password reset failed for user {UserId}: {Errors}", user.Id, errors);
            
            throw new BadRequestException($"Password is not successfully changed. Errors: {errors}");
        }
        
        await cachedService.DeleteAsync(request.Code, cancellationToken);
        
        logger.LogInformation("Password reset successfully for user {UserId}", user.Id);
    }
}