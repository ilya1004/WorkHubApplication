using System.Reflection;
using System.Text;
using ChatService.API.Constants;
using ChatService.API.Filters;
using ChatService.API.Hubs;
using ChatService.API.Middlewares;
using ChatService.API.Services;
using ChatService.API.Settings;
using ChatService.Application.Constants;
using ChatService.Domain.Abstractions.UserContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace ChatService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<GlobalLoggingMiddleware>();
        services.AddTransient<GlobalExceptionHandlingMiddleware>();
        
        services.AddSignalR()
            .AddHubOptions<ChatHub>(options =>
            {
                options.EnableDetailedErrors = true;
                options.AddFilter<GlobalHubLoggingFilter>();
                options.AddFilter<GlobalHubExceptionFilter>();
            });
        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
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

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        {
                            Console.WriteLine($"SignalR access_token: {accessToken}");
                            context.Token = accessToken;
                            return Task.CompletedTask;
                        }

                        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                        {
                            var rawHeader = authHeader.ToString();
                            Console.WriteLine($"Raw Authorization header: {rawHeader}");
                            if (rawHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                var token = rawHeader.Substring("Bearer ".Length).Trim();
                                Console.WriteLine($"Extracted token: {token}");
                                context.Token = token;
                            }
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully");
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

        services.AddHealthChecks();
        
        return services;
    }
}