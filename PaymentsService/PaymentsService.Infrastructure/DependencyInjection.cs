using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Domain.Abstractions.EmployerService;
using PaymentsService.Domain.Abstractions.Repositories;
using PaymentsService.Infrastructure.Data;
using PaymentsService.Infrastructure.Repositories;
using PaymentsService.Infrastructure.Services.StripeServices;
using PaymentsService.Infrastructure.Settings;
using Stripe;

namespace PaymentsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        services.AddScoped<IUnitOfWork, AppUnitOfWork>();
        
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
        var stripeSettings = configuration.GetSection("Stripe").Get<StripeSettings>()!;
        
        StripeConfiguration.ApiKey = stripeSettings.SecretKey;

        services.AddScoped<IEmployerService, StripeEmployerService>();
        services.AddScoped<IFreelancerService, StripeFreelancerService>();
        
        return services;
    }
}