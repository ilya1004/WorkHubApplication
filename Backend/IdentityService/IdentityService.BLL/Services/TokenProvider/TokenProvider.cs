using IdentityService.BLL.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.BLL.Abstractions.TokenProvider;

namespace IdentityService.BLL.Services.TokenProvider;

public class TokenProvider(
    IOptions<JwtSettings> options,
    ILogger<TokenProvider> logger) : ITokenProvider
{
    public string GenerateAccessToken(AppUser user)
    {
        logger.LogInformation("Generating access token for user {UserId}", user.Id);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, user.Role.Name!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            options.Value.Issuer,
            options.Value.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(options.Value.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        logger.LogInformation("Access token generated successfully for user {UserId}", user.Id);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        logger.LogInformation("Generating refresh token");
        
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        logger.LogInformation("Validating expired token");
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = options.Value.Issuer,
            ValidAudience = options.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken is null ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            logger.LogWarning("Invalid token received");
            
            throw new UnauthorizedException("Invalid token");
        }

        logger.LogInformation("Token validated successfully");
        
        return principal;
    }
}