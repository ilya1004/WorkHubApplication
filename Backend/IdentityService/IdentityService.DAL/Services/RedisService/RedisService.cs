using System.Text.Json;
using IdentityService.DAL.Abstractions.RedisService;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityService.DAL.Services.RedisService;

public class RedisService(IDistributedCache distributedCache) : ICachedService
{
    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };

        await distributedCache.SetStringAsync(key, value, options);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await distributedCache.GetStringAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var value = await distributedCache.GetStringAsync(key);
        return value != null;
    }

    public async Task DeleteAsync(string key)
    {
        await distributedCache.RemoveAsync(key);
    }

    public async Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await SetAsync(key, json, expiry);
    }

    public async Task<T?> GetObjectAsync<T>(string key)
    {
        var json = await GetAsync(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }
}