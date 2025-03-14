using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectsService.Application;

public static class DependencyInjection 
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}