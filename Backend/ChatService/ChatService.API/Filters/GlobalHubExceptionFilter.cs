using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Filters;

public class GlobalHubExceptionFilter(
    ILogger<GlobalHubExceptionFilter> logger) : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var hubName = invocationContext.Hub.GetType().Name;
        var methodName = invocationContext.HubMethodName;
        var userId = invocationContext.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        try
        {
            logger.LogInformation("SignalR {Hub}.{Method} invoked by {UserId}", hubName, methodName, userId);

            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SignalR Error in {Hub}.{Method} by {UserId}", hubName, methodName, userId);

            if (invocationContext.Hub is { } hub)
            {
                await hub.Clients.Caller.SendAsync("HandleError", $"Error message: {ex.Message}.");
            }

            return null;
        }
    }
}