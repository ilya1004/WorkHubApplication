using Microsoft.Extensions.Caching.Distributed;
using ProjectsService.Domain.Primitives;

namespace ProjectsService.Infrastructure.Repositories;

public class CachedCommandsRepository<TEntity>(
    ICommandsRepository<TEntity> commandsRepository,
    IDistributedCache distributedCache) : ICommandsRepository<TEntity> where TEntity : Entity
{
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await commandsRepository.AddAsync(entity, cancellationToken);
        await InvalidateCacheAsync(entity.Id);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await commandsRepository.UpdateAsync(entity, cancellationToken);
        await InvalidateCacheAsync(entity.Id);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await commandsRepository.DeleteAsync(entity, cancellationToken);
        await InvalidateCacheAsync(entity.Id);
    }

    private async Task InvalidateCacheAsync(Guid id)
    {
        await distributedCache.RemoveAsync($"{typeof(TEntity).Name}:{id}");
        await distributedCache.RemoveAsync($"{typeof(TEntity).Name}:ListAll");
        await distributedCache.RemoveAsync($"{typeof(TEntity).Name}:CountAll");
    }
}