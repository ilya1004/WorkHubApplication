using ChatService.API;
using ChatService.API.Hubs;
using ChatService.API.Middlewares;
using ChatService.Application;
using ChatService.Domain.Abstractions.AzuriteStartupService;
using ChatService.Domain.Abstractions.DbInitializer;
using ChatService.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddHttpContextAccessor();

services.AddAPI(builder.Configuration);
services.AddApplication(builder.Configuration);
services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var azuriteStartupService = scope.ServiceProvider.GetRequiredService<IAzuriteStartupService>();
    await azuriteStartupService.CreateContainerIfNotExistAsync();

    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.InitializeDbAsync(builder.Configuration);
}

app.UseRouting();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseMiddleware<GlobalLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("hubs/chat");

app.Run();
