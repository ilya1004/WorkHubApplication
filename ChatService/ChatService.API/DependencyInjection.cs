using ChatService.API.Services;
using ChatService.Domain.Abstractions.UserContext;

namespace ChatService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services)
    {
        
        
        services.AddScoped<IUserContext, UserContext>();
        
        return services;
    }
}