using Azure.Storage.Blobs;
using IdentityService.BLL.Services.BlobService;
using IdentityService.BLL.Services.EmailSender;
using IdentityService.BLL.Services.TokenProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using IdentityService.BLL.Abstractions.AzuriteStartupService;
using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.BLL.HealthChecks;
using IdentityService.BLL.Services.AzuriteStartupService;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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

        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IAzuriteStartupService, AzuriteStartupService>();

        services.AddSingleton<IBlobService, BlobService>();
        services.AddSingleton(_ => new BlobServiceClient(configuration.GetConnectionString("AzuriteConnection")));

        services.AddFluentEmail(configuration["EmailSenderMailHog:EmailSender"], configuration["EmailSenderMailHog:SenderName"])
            .AddSmtpSender(configuration["EmailSenderMailHog:Host"], int.Parse(configuration["EmailSenderMailHog:Port"]!));
        
        services.AddHealthChecks()
            .AddAzureBlobStorage(_ => new BlobServiceClient(configuration.GetConnectionString("AzuriteConnection")))
            .AddCheck<SmtpHealthCheck>("smtp_mailhog", HealthStatus.Unhealthy);

        return services;
    }
}