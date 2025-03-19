using System.Reflection;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Infrastructure.GrpcClients;
using PaymentsService.Infrastructure.Interfaces;
using PaymentsService.Infrastructure.Services.StripeAccountsServices;
using PaymentsService.Infrastructure.Services.StripePaymentsServices;
using PaymentsService.Infrastructure.Services.StripeTransfersServices;
using PaymentsService.Infrastructure.Settings;

namespace PaymentsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
        var stripeSettings = configuration.GetSection("StripeSettings").Get<StripeSettings>()!;

        StripeConfiguration.ApiKey = stripeSettings.SecretKey;

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IEmployerAccountsService, StripeEmployerAccountsService>();
        services.AddScoped<IFreelancerAccountsService, StripeFreelancerAccountsService>();
        services.AddScoped<IEmployerPaymentsService, StripeEmployerPaymentsService>();
        services.AddScoped<IPaymentMethodsService, StripePaymentMethodsService>();
        services.AddScoped<ITransfersService, StripeTransfersService>();

        services.AddScoped<IEmployersGrpcClient, EmployersGrpcClient>();
        
        services.Configure<GrpcSettings>(configuration.GetSection("GrpcSettings"));
        var grpcSettings = configuration.GetSection("GrpcSettings").Get<GrpcSettings>()!;

        services.AddGrpcClient<Employers.Employers.EmployersClient>(options =>
        {
            options.Address = new Uri(grpcSettings.IdentityServiceAddress);
        });
        
        services.AddGrpcClient<Freelancers.Freelancers.FreelancersClient>(options =>
        {
            options.Address = new Uri(grpcSettings.IdentityServiceAddress);
        });
        
        services.AddGrpcClient<Projects.Projects.ProjectsClient>(options =>
        {
            options.Address = new Uri(grpcSettings.ProjectsServiceAddress);
        });

        services.AddScoped<IEmployersGrpcClient, EmployersGrpcClient>();
        services.AddScoped<IFreelancersGrpcClient, FreelancersGrpcClient>();
        services.AddScoped<IProjectsGrpcClient, ProjectsGrpcClient>();

        return services;
    }
}