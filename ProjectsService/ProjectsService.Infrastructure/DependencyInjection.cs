using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Repositories;

namespace ProjectsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CommandsDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnectionPrimaryDb")));
        
        services.AddDbContext<QueriesDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnectionReplicaDb")));

        services.AddScoped<IUnitOfWork, AppUnitOfWork>();
            
        return services;
    }
}