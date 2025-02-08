using IdentityService.DAL.Abstractions.Data;
using IdentityService.DAL.Data;
using IdentityService.DAL.Repository;
using IdentityService.DAL.Services.DbInitializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDAL(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        services.AddScoped<IUnitOfWork, AppUnitOfWork>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}
