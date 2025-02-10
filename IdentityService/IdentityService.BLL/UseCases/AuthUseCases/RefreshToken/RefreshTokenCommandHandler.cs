using IdentityService.BLL.DTOs;
using IdentityService.BLL.Services.TokenProvider;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;

public class RefreshTokenCommandHandler(ITokenProvider tokenProvider, UserManager<AppUser> userManager, IConfiguration configuration) : IRequestHandler<RefreshTokenCommand, AuthTokensDTO>
{
    public async Task<AuthTokensDTO> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = tokenProvider.GetPrincipalFromExpiredToken(request.AccessToken);
        var user = await userManager.FindByIdAsync(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault("");

        var newAccessToken = tokenProvider.GenerateAccessToken(user, userRole);
        var newRefreshToken = tokenProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        var expiryDays = Convert.ToInt32(configuration["Jwt:RefreshTokenExpiryDays"]);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await userManager.UpdateAsync(user);

        return new AuthTokensDTO(newAccessToken, newRefreshToken);
    }
}
