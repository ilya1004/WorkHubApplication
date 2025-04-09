using System.Security.Claims;

namespace IdentityService.BLL.Abstractions.TokenProvider;

public interface ITokenProvider
{
    string GenerateAccessToken(AppUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}