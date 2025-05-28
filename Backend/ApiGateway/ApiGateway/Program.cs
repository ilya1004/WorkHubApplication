using ApiGateway;
using ApiGateway.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddTransient<GlobalLoggingMiddleware>();

services.AddLogging(builder.Configuration);
services.AddAuthConfiguration(builder.Configuration);
services.AddYarpConfiguration(builder.Configuration);
services.AddCorsConfiguration(builder.Configuration);

services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();
app.UseCors("AngularApplication");
app.UseRateLimiter();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseMiddleware<GlobalLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();