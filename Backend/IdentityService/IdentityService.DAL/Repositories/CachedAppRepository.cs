using System.Linq.Expressions;
using System.Text.Json;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Primitives;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace IdentityService.DAL.Repositories;

public class CachedAppRepository<TEntity>(
    IRepository<TEntity> repository,
    IDistributedCache distributedCache,
    IOptions<CacheOptions> options) : IRepository<TEntity> where TEntity : Entity
{
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await repository.AddAsync(entity, cancellationToken);
        await InvalidateCacheAsync(entity.Id);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(entity, cancellationToken);
        await InvalidateCacheAsync(entity.Id);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await repository.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        var cacheKey = $"{typeof(TEntity).Name}:{id}";
        var cachedEntity = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedEntity != null) return JsonSerializer.Deserialize<TEntity>(cachedEntity);

        var entity = await repository.GetByIdAsync(id, cancellationToken, includesProperties);

        if (entity != null)
            await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entity), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.RecordExpirationTimeInMinutes)
            }, cancellationToken);

        return entity;
    }

    public async Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{typeof(TEntity).Name}:ListAll";
        var cachedEntities = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedEntities != null) return JsonSerializer.Deserialize<IReadOnlyList<TEntity>>(cachedEntities) ?? [];

        var entities = await repository.ListAllAsync(cancellationToken);

        await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entities), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.RecordExpirationTimeInMinutes)
        }, cancellationToken);

        return entities;
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{typeof(TEntity).Name}:PaginatedListAll:{offset}:{limit}";
        var cachedEntities = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedEntities != null) return JsonSerializer.Deserialize<IReadOnlyList<TEntity>>(cachedEntities) ?? [];

        var entities = await repository.PaginatedListAllAsync(offset, limit, cancellationToken);

        await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entities), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.RecordExpirationTimeInMinutes)
        }, cancellationToken);

        return entities;
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await repository.ListAsync(filter, cancellationToken, includesProperties);
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAsync(Expression<Func<TEntity, bool>>? filter, int offset, int limit,
        CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await repository.PaginatedListAsync(filter, offset, limit, cancellationToken, includesProperties);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await repository.UpdateAsync(entity, cancellationToken);
        await InvalidateCacheAsync(entity.Id);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await repository.AnyAsync(filter, cancellationToken);
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{typeof(TEntity).Name}:CountAll";
        var cachedCount = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedCount != null) return int.Parse(cachedCount);

        var count = await repository.CountAllAsync(cancellationToken);

        await distributedCache.SetStringAsync(cacheKey, count.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.RecordExpirationTimeInMinutes)
        }, cancellationToken);

        return count;
    }

    private async Task InvalidateCacheAsync(Guid id)
    {
        var cacheKey = $"{typeof(TEntity).Name}:{id}";
        await distributedCache.RemoveAsync(cacheKey);
    }
}