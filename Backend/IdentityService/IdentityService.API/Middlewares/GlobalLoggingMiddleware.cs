using System.Diagnostics;

namespace IdentityService.API.Middlewares;

public class GlobalLoggingMiddleware(
    ILogger<GlobalLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await next(context);
            
        stopwatch.Stop();
            
        logger.LogInformation(
            "HTTP {Method} {Path}{Query} => {StatusCode} in {Duration}ms",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}