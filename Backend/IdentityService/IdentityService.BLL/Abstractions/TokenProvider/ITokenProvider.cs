using System.Security.Claims;

namespace IdentityService.BLL.Services.TokenProvider;

public interface ITokenProvider
{
    string GenerateAccessToken(AppUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}