namespace IdentityService.BLL.Settings;

public class JwtSettings
{
    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int RefreshTokenExpiryDays { get; init; }
    public int AccessTokenExpiryMinutes { get; init; }
}