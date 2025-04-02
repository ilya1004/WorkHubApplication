using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Filters;

public class GlobalHubLoggingFilter(
    ILogger<GlobalHubLoggingFilter> logger) : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var startTime = DateTime.UtcNow;
        
        var result = await next(context);
        
        var duration = DateTime.UtcNow - startTime;
        
        logger.LogInformation(
            "SignalR {Hub}.{Method} executed in {Duration}ms",
            context.Hub.GetType().Name,
            context.HubMethodName,
            duration.TotalMilliseconds);
            
        return result;
    }
}