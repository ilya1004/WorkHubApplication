using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PaymentsService.API.Middlewares;
using PaymentsService.API.Services;
using PaymentsService.API.Settings;
using PaymentsService.Application.Constants;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<GlobalLoggingMiddleware>();
        services.AddTransient<GlobalExceptionHandlingMiddleware>();
        
        var jwtSettings = configuration.GetRequiredSection("JwtSettings").Get<JwtSettings>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.AdminPolicy, policy =>
            {
                policy.RequireRole(AppRoles.AdminRole);
            })
            .AddPolicy(AuthPolicies.FreelancerPolicy, policy =>
            {
                policy.RequireRole(AppRoles.FreelancerRole);
            })
            .AddPolicy(AuthPolicies.EmployerPolicy, policy =>
            {
                policy.RequireRole(AppRoles.EmployerRole);
            });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}