using System.Reflection;
using ChatService.API.Constants;
using ChatService.API.Services;
using ChatService.API.Settings;
using ChatService.Applications.Constants;
using ChatService.Domain.Abstractions.UserContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ChatService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        var jwtSettings = configuration.GetRequiredSection("JwtSettings").Get<JwtSettings>();
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options => 
            {
                options.Authority = "https://localhost:7080";
                options.RequireHttpsMetadata = false;
                options.Audience = jwtSettings!.Audience;
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.AdminPolicy, policy =>
            {
                policy.RequireRole(AppRoles.AdminRole);
            });
        
        services.AddScoped<IUserContext, UserContext>();
        
        return services;
    }
}