using Confluent.Kafka;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProjectsService.Domain.Abstractions.KafkaProducerServices;
using ProjectsService.Domain.Abstractions.StartupServices;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Repositories;
using ProjectsService.Infrastructure.Services.DbStartupService;
using ProjectsService.Infrastructure.Services.HangfireJobsInitializer;
using ProjectsService.Infrastructure.Services.KafkaConsumerServices;
using ProjectsService.Infrastructure.Services.KafkaProducerServices;
using ProjectsService.Infrastructure.Services.LogstashHelpers;
using Serilog;

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

        services.AddOptionsWithValidateOnStart<CacheOptions>()
            .BindConfiguration("CacheOptions");

        services.AddScoped<IRecurringJobManager, RecurringJobManager>();
        services.AddScoped<IDbStartupService, DbStartupService>();
        services.AddScoped<IBackgroundJobsInitializer, HangfireJobsInitializer>();
        
        services.AddOptionsWithValidateOnStart<KafkaSettings>()
            .BindConfiguration("KafkaSettings");

        services.AddSingleton<IPaymentsProducerService, PaymentsProducerService>();

        services.AddHostedService<PaymentsConsumerService>();
        
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgresConnectionPrimaryDb")!, name: "postgres-primary")
            .AddNpgSql(configuration.GetConnectionString("PostgresConnectionReplicaDb")!, name: "postgres-replica")
            .AddNpgSql(configuration.GetConnectionString("PostgresConnectionHangfireDb")!, name: "postgres-hangfire")
            .AddRedis(configuration.GetConnectionString("RedisConnection")!)
            .AddKafka(new ProducerConfig
            {
                BootstrapServers = configuration["KafkaSettings:BootstrapServers"]
            }, name: "kafka")
            .AddElasticsearch(
                elasticsearchUri: configuration["Elasticsearch:Url"]!,
                name: "elasticsearch",
                failureStatus: HealthStatus.Unhealthy);
        
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(new LogstashTextFormatter())
            .WriteTo.Http(
                requestUri: configuration["Logstash:Url"]!, 
                queueLimitBytes: null,
                textFormatter: new LogstashTextFormatter(),
                httpClient: new LogstashHttpClient()
            )
            .CreateLogger();

        services.AddLogging(logging => logging.AddSerilog());
        
        return services;
    }
}