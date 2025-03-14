namespace IdentityService.DAL.Abstractions.RedisService;

public interface ICachedService
{
    Task SetAsync(string key, string value, TimeSpan? expiry = null);
    Task<string?> GetAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task DeleteAsync(string key);
    Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetObjectAsync<T>(string key);
}