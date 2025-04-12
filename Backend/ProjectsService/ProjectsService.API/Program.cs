using System.Text.Json.Serialization;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ProjectsService.API;
using ProjectsService.API.GrpcServices;
using ProjectsService.API.Middlewares;
using ProjectsService.Application;
using ProjectsService.Domain.Abstractions.StartupServices;
using ProjectsService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

services.AddHttpContextAccessor();

services.AddAPI(builder.Configuration);
services.AddApplication();
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
    var dbStartupService = scope.ServiceProvider.GetRequiredService<IDbStartupService>();
    await dbStartupService.MakeMigrationsAsync();
    await dbStartupService.InitializeDb();
    
    var jobInitializer = scope.ServiceProvider.GetRequiredService<IBackgroundJobsInitializer>();
    jobInitializer.StartBackgroundJobs();
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

app.MapGrpcService<ProjectsGrpcService>();

app.MapControllers();

app.Run();
