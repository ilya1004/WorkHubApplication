using PaymentsService.API.Services;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services)
    {

        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}