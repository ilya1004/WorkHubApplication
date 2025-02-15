using System.Reflection;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace ProjectsService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation(config =>
        {
            config.EnableFormBindingSourceAutomaticValidation = true;
            config.EnableBodyBindingSourceAutomaticValidation = true;
            config.EnableQueryBindingSourceAutomaticValidation = true;
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}