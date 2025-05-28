using System.Text.Json;
using IdentityService.DAL.Abstractions.RedisService;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityService.DAL.Services.RedisService;

public class RedisService(IDistributedCache distributedCache) : ICachedService
{
    public async Task SetAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };

        await distributedCache.SetStringAsync(key, value, options, cancellationToken);
    }

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return await distributedCache.GetStringAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await distributedCache.GetStringAsync(key, cancellationToken);
        return value != null;
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);
        await SetAsync(key, json, expiry, cancellationToken);
    }

    public async Task<T?> GetObjectAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await GetAsync(key, cancellationToken);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }
}