namespace IdentityService.DAL.Abstractions.RedisService;

public interface ICachedService
{
    Task SetAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
    Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task<T?> GetObjectAsync<T>(string key, CancellationToken cancellationToken = default);
}