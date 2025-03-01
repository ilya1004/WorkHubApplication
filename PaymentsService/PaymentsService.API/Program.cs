using PaymentsService.API.Middlewares;
using PaymentsService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddTransient<GlobalExceptionHandlingMiddleware>();
    
services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.Run();