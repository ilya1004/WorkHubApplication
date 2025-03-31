using ApiGateway;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAuthConfiguration(builder.Configuration);
services.AddYarpConfiguration(builder.Configuration);

services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();
app.UseRateLimiter();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();