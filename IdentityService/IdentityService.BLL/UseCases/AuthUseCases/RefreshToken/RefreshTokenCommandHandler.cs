using IdentityService.BLL.DTOs;
using IdentityService.BLL.Services.TokenProvider;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokensDTO>
{
    private readonly ITokenProvider _tokenProvider;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(ITokenProvider tokenProvider, UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _tokenProvider = tokenProvider;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthTokensDTO> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenProvider.GetPrincipalFromExpiredToken(request.AccessToken);
        var user = await _userManager.FindByIdAsync(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault("");

        var newAccessToken = _tokenProvider.GenerateAccessToken(user, userRole);
        var newRefreshToken = _tokenProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        var expiryDays = Convert.ToInt32(_configuration["Jwt:RefreshTokenExpiryDays"]);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await _userManager.UpdateAsync(user);

        return new AuthTokensDTO(newAccessToken, newRefreshToken);
    }
}
