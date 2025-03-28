using IdentityService.DAL.Abstractions.DbStartupService;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using IdentityService.DAL.Repositories;
using IdentityService.DAL.Services.DbStartupService;
using IdentityService.DAL.Services.RedisService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddOptionsWithValidateOnStart<CacheOptions>()
            .BindConfiguration("CacheOptions");

        services.AddScoped<IUnitOfWork, AppUnitOfWork>();
        services.AddScoped<ICachedService, RedisService>();
        services.AddScoped<IDbStartupService, DbStartupService>();

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgresConnection")!)
            .AddRedis(configuration.GetConnectionString("RedisConnection")!);

        return services;
    }
}