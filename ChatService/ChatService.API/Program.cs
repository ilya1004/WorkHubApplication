using ChatService.API;
using ChatService.API.Hubs;
using ChatService.API.Middlewares;
using ChatService.Applications;
using ChatService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddTransient<GlobalExceptionHandlingMiddleware>();
services.AddSignalR();
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

app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapHub<ChatHub>("hubs/chat");

app.Run();
