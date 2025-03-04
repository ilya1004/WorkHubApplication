using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectsService.Application.Abstractions.BackgroundJobs;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Abstractions.StartupServices;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Repositories;
using ProjectsService.Infrastructure.Services.HangfireJobsInitializer;
using ProjectsService.Infrastructure.Services.HangfireScheduler;

namespace ProjectsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CommandsDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnectionPrimaryDb")));
        
        services.AddDbContext<QueriesDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnectionReplicaDb")));

        services.AddScoped<IUnitOfWork, AppUnitOfWork>();
        
        services.AddHangfire(config => 
            config.UsePostgreSqlStorage(options => 
                options.UseNpgsqlConnection(configuration.GetConnectionString("PostgresConnectionHangfireDb"))));
        
        services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(10));

        services.AddScoped<IBackgroundJobScheduler, HangfireScheduler>();
        services.AddScoped<IBackgroundJobsInitializer, HangfireJobsInitializer>();
        
        return services;
    }
}