using Azure.Storage.Blobs;
using IdentityService.BLL.Services.BlobService;
using IdentityService.BLL.Services.EmailSender;
using IdentityService.BLL.Services.TokenProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Confluent.Kafka;
using IdentityService.BLL.Abstractions.AzuriteStartupService;
using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.BLL.Abstractions.TokenProvider;
using IdentityService.BLL.HealthChecks;
using IdentityService.BLL.Services.AzuriteStartupService;
using IdentityService.BLL.Services.KafkaConsumerServices;
using IdentityService.BLL.Services.LogstashHelpers;
using IdentityService.BLL.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace IdentityService.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBLL(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        services.AddOptionsWithValidateOnStart<AzuriteSettings>()
            .BindConfiguration("AzuriteSettings");

        services.AddOptionsWithValidateOnStart<KafkaSettings>()
            .BindConfiguration("KafkaSettings");
        
        var azuriteSettings = configuration.GetRequiredSection("AzuriteSettings").Get<AzuriteSettings>()!;
        var kafkaSettings = configuration.GetRequiredSection("KafkaSettings").Get<KafkaSettings>()!;

        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IAzuriteStartupService, AzuriteStartupService>();

        services.AddSingleton<IBlobService, BlobService>();
        services.AddSingleton(_ => new BlobServiceClient(azuriteSettings.ConnectionString));

        services.AddFluentEmail(configuration["EmailSenderMailHog:EmailSender"], 
                configuration["EmailSenderMailHog:SenderName"])
            .AddSmtpSender(configuration["EmailSenderMailHog:Host"], 
                int.Parse(configuration["EmailSenderMailHog:Port"]!));

        services.AddHostedService<EmployerAccountsConsumerService>();
        services.AddHostedService<FreelancerAccountsConsumerService>();
        
        services.AddHealthChecks()
            .AddAzureBlobStorage(_ => new BlobServiceClient(azuriteSettings.ConnectionString))
            .AddCheck<SmtpHealthCheck>("smtp_mailhog", HealthStatus.Unhealthy)
            .AddKafka(new ProducerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers
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