using PaymentsService.API;
using PaymentsService.API.Middlewares;
using PaymentsService.Applications;
using PaymentsService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddHttpContextAccessor();
services.AddTransient<GlobalExceptionHandlingMiddleware>();

services.AddAPI();
services.AddApplication();
services.AddInfrastructure(builder.Configuration);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();