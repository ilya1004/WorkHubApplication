using System.Security.Claims;
using ChatService.Application.Exceptions;
using ChatService.Domain.Abstractions.UserContext;

namespace ChatService.API.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid GetUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (userId is null)
        {
            throw new UnauthorizedException("You are not authorized to access this resource.");    
        }

        return Guid.Parse(userId);
    }
    
    public string GetUserRole()
    {
        var userRole = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        
        if (userRole is null)
        {
            throw new UnauthorizedException("You are not authorized to access this resource.");    
        }

        return userRole;
    }
}