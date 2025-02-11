using IdentityService.DAL.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IdentityService.API.AuthorizationPolicies.AdminOrSelfPolicy;

public class AdminOrSelfHandler : AuthorizationHandler<AdminOrSelfRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrSelfRequirement requirement)
    {
        var user = context.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Task.CompletedTask;
        }

        var userRoleClaim = user.FindFirstValue(ClaimTypes.Role);
        if (userRoleClaim == AppRoles.AdminRole)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var routeData = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;
        var routeUserId = routeData?.HttpContext.Request.RouteValues["userId"]?.ToString();

        if (routeUserId == userIdClaim)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
