using System.Linq.Expressions;
using System.Text.Json;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace IdentityService.DAL.Repositories;

public class CachedUsersRepository(
    IUsersRepository usersRepository, 
    IDistributedCache distributedCache,
    IOptions<CacheOptions> options) : IUsersRepository
{
    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, 
        params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        var cacheKey = $"{nameof(AppUser)}:{id}";
        var cachedUser = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedUser != null)
        {
            return JsonSerializer.Deserialize<AppUser>(cachedUser);
        }

        var user = await usersRepository.GetByIdAsync(id, cancellationToken, includesProperties);

        if (user != null)
        {
            await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user), new DistributedCacheEntryOptions
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

    public async Task<IReadOnlyList<AppUser>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await usersRepository.PaginatedListAllAsync(offset, limit, cancellationToken);
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
        var cacheKey = $"{nameof(AppUser)}:{userId}";
        await distributedCache.RemoveAsync(cacheKey);
    }
}