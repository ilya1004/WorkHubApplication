using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.Repositories;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Infrastructure.Data;
using PaymentsService.Infrastructure.Repositories;
using PaymentsService.Infrastructure.Services.StripeAccountsServices;
using PaymentsService.Infrastructure.Services.StripePaymentsServices;
using PaymentsService.Infrastructure.Services.StripeTransfersServices;
using PaymentsService.Infrastructure.Settings;

namespace PaymentsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        services.AddScoped<IUnitOfWork, AppUnitOfWork>();
        
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
        var stripeSettings = configuration.GetSection("StripeSettings").Get<StripeSettings>()!;
        
        StripeConfiguration.ApiKey = stripeSettings.SecretKey;

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IEmployerAccountsService, StripeEmployerAccountsService>();
        services.AddScoped<IFreelancerAccountsService, StripeFreelancerAccountsService>();
        services.AddScoped<IEmployerPaymentsService, StripeEmployerPaymentsService>();
        services.AddScoped<ITransfersService, StripeTransfersService>();
        
        return services;
    }
}