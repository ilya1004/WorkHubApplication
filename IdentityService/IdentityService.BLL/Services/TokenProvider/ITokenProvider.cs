using System.Security.Claims;

namespace IdentityService.BLL.Services.TokenProvider;

public interface ITokenProvider
{
    string GenerateAccessToken(AppUser user, string roleName);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
