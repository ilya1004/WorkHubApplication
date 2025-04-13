using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using IdentityService.DAL.Abstractions.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace IdentityService.DAL.Repositories;

public class CachedUsersRepository(
    IUsersRepository usersRepository,
    IDistributedCache distributedCache,
    IOptions<CacheOptions> options) : IUsersRepository
{
    private readonly JsonSerializerOptions _jsonOptions = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };
    
    public async Task<AppUser?> GetByIdAsync(Guid id, bool withTracking = true, CancellationToken cancellationToken = default,
        params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        if (includesProperties is not null && includesProperties.Length > 0)
        {
            return await usersRepository.GetByIdAsync(id, withTracking, cancellationToken, includesProperties);
        }
        
        var cacheKey = $"{nameof(AppUser)}:{id}";
        var cachedUser = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedUser != null)
        {
            return JsonSerializer.Deserialize<AppUser>(cachedUser, _jsonOptions);
        }

        var user = await usersRepository.GetByIdAsync(id, withTracking, cancellationToken, includesProperties);

        if (user != null)
        {
            await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user, _jsonOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.RecordExpirationTimeInMinutes)
            }, cancellationToken);
        }

        return user;
    }
    
    public async Task<AppUser?> FirstOrDefaultAsync(Expression<Func<AppUser, bool>> filter, CancellationToken cancellationToken = default,
        params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        return await usersRepository.FirstOrDefaultAsync(filter, cancellationToken, includesProperties);
    }

    public async Task<IReadOnlyList<AppUser>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default,
        params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        return await usersRepository.PaginatedListAllAsync(offset, limit, cancellationToken, includesProperties);
    }

    public async Task<IReadOnlyList<AppUser>> PaginatedListAsync(Expression<Func<AppUser, bool>>? filter, int offset, int limit,
        CancellationToken cancellationToken = default, params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        return await usersRepository.PaginatedListAsync(filter, offset, limit, cancellationToken, includesProperties);
    }

    public async Task UpdateAsync(AppUser entity, CancellationToken cancellationToken = default)
    {
        await usersRepository.UpdateAsync(entity, cancellationToken);
        await InvalidateCacheAsync(entity.Id);
    }

    public async Task DeleteAsync(AppUser entity, CancellationToken cancellationToken = default)
    {
        await usersRepository.DeleteAsync(entity, cancellationToken);
        await InvalidateCacheAsync(entity.Id);
    }

    public async Task<int> CountAsync(Expression<Func<AppUser, bool>>? filter, CancellationToken cancellationToken = default)
    {
        return await usersRepository.CountAsync(filter, cancellationToken);
    }

    private async Task InvalidateCacheAsync(Guid userId)
    {
        await distributedCache.RemoveAsync($"{nameof(AppUser)}:{userId}");
    }
}