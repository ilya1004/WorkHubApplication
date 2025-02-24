using ChatService.API;
using ChatService.API.Hubs;
using ChatService.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddTransient<GlobalExceptionHandlingMiddleware>();
services.AddSignalR();
services.AddHttpContextAccessor();

services.AddAPI();


var app = builder.Build();

app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapHub<ChatHub>("/Chat");

app.Run();
