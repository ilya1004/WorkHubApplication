using IdentityService.DAL.Abstractions.DbInitializer;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using IdentityService.DAL.Repository;
using IdentityService.DAL.Services.DbInitializer;
using IdentityService.DAL.Services.RedisService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace IdentityService.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDAL(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
        });
        
        services.AddScoped<IUnitOfWork, AppUnitOfWork>();
        services.AddScoped<ICachedService, RedisService>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}