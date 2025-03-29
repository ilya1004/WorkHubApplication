namespace ChatService.API.Middlewares;

public class GlobalLoggingMiddleware(
    ILogger<GlobalLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var startTime = DateTime.UtcNow;
        
        await next(context);
            
        var duration = DateTime.UtcNow - startTime;
            
        logger.LogInformation(
            "HTTP {Method} {Path}{Query} => {StatusCode} in {Duration}ms",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            context.Response.StatusCode,
            duration.TotalMilliseconds);
    }
}