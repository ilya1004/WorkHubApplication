using ProjectsService.API;
using ProjectsService.API.Middlewares;
using ProjectsService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddTransient<GlobalExceptionHandlingMiddleware>();

services.AddAPI(builder.Configuration);

services.AddInfrastructure(builder.Configuration);
    
services.AddEndpointsApiExplorer()
    .AddSwaggerGen();

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
