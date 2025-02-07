using IdentityService.BLL.Services.EmailSender;
using IdentityService.BLL.Services.EmailVerificationCodeService;
using IdentityService.BLL.Services.TokenProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IdentityService.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBLL(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IEmailSender, EmailSender>();

        services.AddFluentEmail(configuration["EmailSenderMailHog:EmailSender"], configuration["EmailSenderMailHog:SenderName"])
                .AddSmtpSender(configuration["EmailSenderMailHog:Host"], int.Parse(configuration["EmailSenderMailHog:Port"]!));

        return services;
    }
}
