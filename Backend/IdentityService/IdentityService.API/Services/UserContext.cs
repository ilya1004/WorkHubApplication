using System.Security.Claims;
using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.Exceptions;

namespace IdentityService.API.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid GetUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null) throw new UnauthorizedException("You are not authorized to access this resource.");

        return Guid.Parse(userId);
    }
}