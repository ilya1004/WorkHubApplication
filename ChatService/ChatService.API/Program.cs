using ChatService.API;
using ChatService.API.Filters;
using ChatService.API.Hubs;
using ChatService.API.Middlewares;
using ChatService.Applications;
using ChatService.Domain.Abstractions.DbInitializer;
using ChatService.Infrastructure;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddTransient<GlobalExceptionHandlingMiddleware>();

services.AddSignalR()
    .AddHubOptions<ChatHub>(options =>
    {
        options.EnableDetailedErrors = true;
        options.AddFilter<GlobalHubExceptionFilter>();
    });

services.AddHttpContextAccessor();

services.AddAPI(builder.Configuration);
services.AddApplication(builder.Configuration);
services.AddInfrastructure(builder.Configuration);

services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.InitializeDbAsync(builder.Configuration);
}

app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapControllers();
app.MapHub<ChatHub>("hubs/chat");

app.Run();
