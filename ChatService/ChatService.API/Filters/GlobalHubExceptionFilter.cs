using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Filters;

public class GlobalHubExceptionFilter(ILogger<GlobalHubExceptionFilter> logger) : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            if (invocationContext.Hub is { } hub)
            {
                await hub.Clients.Caller.SendAsync("HandleError", $"Error message: {ex.Message}.");
            }

            return null;
        }
    }
}