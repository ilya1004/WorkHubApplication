using System.Security.Claims;
using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.API.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid GetUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null) throw new UnauthorizedException("You are not authorized to access this resource.");

        return Guid.Parse(userId);
    }

    public string GetUserRole()
    {
        var userRole = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

        if (userRole is null) throw new UnauthorizedException("You are not authorized to access this resource.");

        return userRole;
    }

    public string GetUserEmail()
    {
        var userEmail = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        if (userEmail is null) throw new UnauthorizedException("You are not authorized to access this resource.");

        return userEmail;
    }
}