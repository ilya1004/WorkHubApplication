using IdentityService.DAL.Models;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace IdentityService.DAL.Services.TokenCacheService;

public class TokenCacheService : ITokenCacheService
{
    private readonly IDatabase _db;
    private readonly TimeSpan _tokenLifetime;

    public TokenCacheService(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _db = redis.GetDatabase();
        _tokenLifetime = TimeSpan.FromHours(int.Parse(configuration["EmailVerificationTokenLifetimeInHours"]!));
    }

    public async Task SaveTokenAsync(EmailVerificationToken token)
    {
        var tokenJson = JsonSerializer.Serialize(token);
        await _db.StringSetAsync($"email_verification:{token.Id}", tokenJson, _tokenLifetime);
    }

    public async Task<EmailVerificationToken?> GetTokenAsync(Guid tokenId)
    {
        var tokenJson = await _db.StringGetAsync($"email_verification:{tokenId}");
        return tokenJson.IsNullOrEmpty ? null : JsonSerializer.Deserialize<EmailVerificationToken>(tokenJson!);
    }

    public async Task<bool> DeleteTokenAsync(Guid tokenId)
    {
        return await _db.KeyDeleteAsync($"email_verification:{tokenId}");
    }
}
