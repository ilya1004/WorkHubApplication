using IdentityService.DAL.Abstractions.RedisService;

namespace IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;

public class ResetPasswordCommandHandler(
    UserManager<AppUser> userManager,
    ICachedService cachedService) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null) throw new NotFoundException("User with this email does not exist.");

        var token = await cachedService.GetAsync(request.Code);
        
        if (string.IsNullOrEmpty(token)) throw new BadRequestException("Invalid resetting password code.");
        
        var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Password is not successfully changed. Errors: {errors}");
        }
        
        await cachedService.DeleteAsync(request.Code);
    }
}