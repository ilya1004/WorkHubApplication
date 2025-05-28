using System.Reflection;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Infrastructure.HealthChecks;
using PaymentsService.Infrastructure.Services.KafkaConsumerServices;
using PaymentsService.Infrastructure.Services.KafkaProducerServices;
using PaymentsService.Infrastructure.GrpcClients;
using PaymentsService.Infrastructure.Interceptors;
using PaymentsService.Infrastructure.Interfaces;
using PaymentsService.Infrastructure.Services.LogstashHelpers;
using PaymentsService.Infrastructure.Services.StripeAccountsServices;
using PaymentsService.Infrastructure.Services.StripePaymentsServices;
using PaymentsService.Infrastructure.Services.StripeTransfersServices;
using Serilog;

namespace PaymentsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<StripeSettings>()
            .BindConfiguration("StripeSettings");
        
        var stripeSettings = configuration.GetRequiredSection("StripeSettings").Get<StripeSettings>()!;

        StripeConfiguration.ApiKey = stripeSettings.SecretKey;

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IEmployerAccountsService, StripeEmployerAccountsService>();
        services.AddScoped<IFreelancerAccountsService, StripeFreelancerAccountsService>();
        services.AddScoped<IEmployerPaymentsService, StripeEmployerPaymentsService>();
        services.AddScoped<IPaymentMethodsService, StripePaymentMethodsService>();
        services.AddScoped<ITransfersService, StripeTransfersService>();
        
        services.AddOptionsWithValidateOnStart<GrpcSettings>()
            .BindConfiguration("GrpcSettings");
        
        var grpcSettings = configuration.GetRequiredSection("GrpcSettings").Get<GrpcSettings>()!;

        services.AddSingleton<GrpcLoggingInterceptor>();
        services.AddSingleton<AuthInterceptor>();
        
        services.AddGrpcClient<Employers.Employers.EmployersClient>(options =>
            {
                options.Address = new Uri(grpcSettings.IdentityServiceAddress);
            })
            .AddInterceptor<GrpcLoggingInterceptor>()
            .AddInterceptor<AuthInterceptor>();
        
        services.AddGrpcClient<Freelancers.Freelancers.FreelancersClient>(options =>
            {
                options.Address = new Uri(grpcSettings.IdentityServiceAddress);
            })
            .AddInterceptor<GrpcLoggingInterceptor>()
            .AddInterceptor<AuthInterceptor>();
        
        services.AddGrpcClient<Projects.Projects.ProjectsClient>(options => 
            { 
                options.Address = new Uri(grpcSettings.ProjectsServiceAddress); 
            })
            .AddInterceptor<GrpcLoggingInterceptor>()
            .AddInterceptor<AuthInterceptor>();

        services.AddScoped<IEmployersGrpcClient, EmployersGrpcClient>();
        services.AddScoped<IFreelancersGrpcClient, FreelancersGrpcClient>();
        services.AddScoped<IProjectsGrpcClient, ProjectsGrpcClient>();

        services.AddOptionsWithValidateOnStart<KafkaSettings>()
            .BindConfiguration("KafkaSettings");
        
        var kafkaSettings = configuration.GetRequiredSection("KafkaSettings").Get<KafkaSettings>()!;
        
        services.AddSingleton<IAccountsProducerService, AccountsProducerService>();
        services.AddSingleton<IPaymentsProducerService, PaymentsProducerService>();
        
        services.AddHostedService<PaymentsConsumerService>();

        services.AddHealthChecks()
            .AddCheck<StripeHealthCheck>("stipe", HealthStatus.Unhealthy)
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