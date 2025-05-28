using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ProjectsService.Domain.Abstractions.Specification;
using ProjectsService.Domain.Primitives;

namespace ProjectsService.Infrastructure.Repositories;

public class CachedQueriesRepository<TEntity>(
    IQueriesRepository<TEntity> queriesRepository,
    IDistributedCache distributedCache,
    IOptions<CacheOptions> options) : IQueriesRepository<TEntity> where TEntity : Entity
{
    private readonly JsonSerializerOptions _serializerOptions = new() 
        { ReferenceHandler = ReferenceHandler.IgnoreCycles };
    public async Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{typeof(TEntity).Name}:ListAll";
        var cachedEntities = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedEntities != null)
        {
            return JsonSerializer.Deserialize<IReadOnlyList<TEntity>>(cachedEntities) ?? [];
        }

        var entities = await queriesRepository.ListAllAsync(cancellationToken);

        await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entities, _serializerOptions), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.RecordExpirationTimeInMinutes)
        }, cancellationToken);

        return entities;
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default, 
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await queriesRepository.PaginatedListAllAsync(offset, limit, cancellationToken, includesProperties);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? filter, CancellationToken cancellationToken = default, 
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await queriesRepository.ListAsync(filter, cancellationToken, includesProperties);
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAsync(Expression<Func<TEntity, bool>>? filter, int offset, int limit, 
        CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        return await queriesRepository.PaginatedListAsync(filter, offset, limit, cancellationToken, includesProperties);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        if (includesProperties is not null && includesProperties.Length > 0)
        {
            return await queriesRepository.GetByIdAsync(id, cancellationToken, includesProperties);    
        }
        
        var cacheKey = $"{typeof(TEntity).Name}:{id}";
        var cachedEntity = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedEntity != null)
        {
            return JsonSerializer.Deserialize<TEntity>(cachedEntity);
        }

        var entity = await queriesRepository.GetByIdAsync(id, cancellationToken, includesProperties);

        if (entity != null)
        {
            await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(entity, _serializerOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.RecordExpirationTimeInMinutes)
            }, cancellationToken);
        }

        return entity;
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await queriesRepository.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await queriesRepository.AnyAsync(filter, cancellationToken);
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{typeof(TEntity).Name}:CountAll";
        var cachedCount = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedCount != null)
        {
            return int.Parse(cachedCount);
        }

        var count = await queriesRepository.CountAllAsync(cancellationToken);

        await distributedCache.SetStringAsync(cacheKey, count.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.RecordExpirationTimeInMinutes)
        }, cancellationToken);

        return count;
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await queriesRepository.CountAsync(filter, cancellationToken);
    }

    public async Task<int> CountByFilterAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await queriesRepository.CountByFilterAsync(specification, cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetByFilterAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await queriesRepository.GetByFilterAsync(specification, cancellationToken);
    }
}