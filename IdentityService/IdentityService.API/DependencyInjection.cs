using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services)
    {
        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddHttpContextAccessor();
        services.AddScoped<SignInManager<AppUser>>();

        return services;
    }
}
