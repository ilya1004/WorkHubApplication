using System.Text.Json.Serialization;
using Hangfire;
using ProjectsService.API;
using ProjectsService.API.Middlewares;
using ProjectsService.Application;
using ProjectsService.Domain.Abstractions.StartupServices;
using ProjectsService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

services.AddTransient<GlobalExceptionHandlingMiddleware>();

services.AddHttpContextAccessor();

services.AddAPI(builder.Configuration);
services.AddApplication(builder.Configuration);
services.AddInfrastructure(builder.Configuration);

services.AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHangfireDashboard();
}

using (var scope = app.Services.CreateScope())
{
    var jobInitializer = scope.ServiceProvider.GetRequiredService<IBackgroundJobsInitializer>();
    jobInitializer.StartBackgroundJobs();
}

app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
