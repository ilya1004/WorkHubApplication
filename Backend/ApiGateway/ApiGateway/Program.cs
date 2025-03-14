using ApiGateway;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAuthConfiguration(builder.Configuration);
services.AddYarpConfiguration(builder.Configuration);

var app = builder.Build();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();