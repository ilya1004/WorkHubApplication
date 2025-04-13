using IdentityService.DAL.Constants;
using System.Security.Claims;

namespace IdentityService.API.AuthorizationPolicies.AdminOrSelfPolicy;

public class AdminOrSelfHandler(
    IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<AdminOrSelfRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrSelfRequirement requirement)
    {
        var user = context.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var userRoleClaim = user.FindFirstValue(ClaimTypes.Role);
        if (userRoleClaim == AppRoles.AdminRole)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (httpContextAccessor.HttpContext!.Request.RouteValues.TryGetValue("userId", out var routeUserId))
        {
            if (routeUserId is null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var userId = routeUserId.ToString();

            if (userId == userIdClaim) context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}