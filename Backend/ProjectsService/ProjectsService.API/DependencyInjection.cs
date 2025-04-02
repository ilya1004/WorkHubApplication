using System.Reflection;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectsService.API.Constants;
using ProjectsService.API.Interceptors;
using ProjectsService.API.Middlewares;
using ProjectsService.API.Services;
using ProjectsService.API.Settings;
using ProjectsService.Application.Constants;
using ProjectsService.Application.Settings;
using ProjectsService.Domain.Abstractions.UserContext;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace ProjectsService.API;

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
            })
            .AddPolicy(AuthPolicies.FreelancerOrEmployerPolicy, policy =>
            {
                policy.RequireRole(AppRoles.FreelancerRole, AppRoles.EmployerRole);
            })
            .AddPolicy(AuthPolicies.AdminOrEmployerPolicy, policy =>
            {
                policy.RequireRole(AppRoles.AdminRole, AppRoles.EmployerRole);
            })
            .AddPolicy(AuthPolicies.AdminOrFreelancerPolicy, policy =>
            {
                policy.RequireRole(AppRoles.AdminRole, AppRoles.FreelancerRole);
            });
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation(config =>
        {
            config.EnableFormBindingSourceAutomaticValidation = true;
            config.EnableBodyBindingSourceAutomaticValidation = true;
            config.EnableQueryBindingSourceAutomaticValidation = true;
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddOptionsWithValidateOnStart<ProjectsSettings>()
            .BindConfiguration("ProjectsSettings");

        services.AddScoped<IUserContext, UserContext>();

        services.AddSingleton<GrpcLoggingInterceptor>();
        services.AddSingleton<ErrorHandlingInterceptor>();
        
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
            options.Interceptors.Add<GrpcLoggingInterceptor>();
            options.Interceptors.Add<ErrorHandlingInterceptor>();
        });
        
        return services;
    }
}