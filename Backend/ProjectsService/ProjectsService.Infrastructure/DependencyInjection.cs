using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectsService.Domain.Abstractions.StartupServices;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Repositories;
using ProjectsService.Infrastructure.Services.HangfireJobsInitializer;
using ProjectsService.Infrastructure.Settings;

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
        
        services.AddHangfireServer();
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
        });

        services.Configure<CacheOptions>(configuration.GetSection("CacheOptions"));

        services.AddScoped<IRecurringJobManager, RecurringJobManager>();
        services.AddScoped<IBackgroundJobsInitializer, HangfireJobsInitializer>();
        
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgresConnectionHangfireDb")!, name: "postgres-hangfire")
            .AddRedis(configuration.GetConnectionString("RedisConnection")!);
        
        return services;
    }
}