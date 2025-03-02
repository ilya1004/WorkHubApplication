using PaymentsService.API;
using PaymentsService.API.Middlewares;
using PaymentsService.Applications;
using PaymentsService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHttpContextAccessor();
services.AddTransient<GlobalExceptionHandlingMiddleware>();

services.AddAPI();
services.AddApplication();
services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();