using ProjectsService.Domain.Primitives;

namespace ProjectsService.Domain.Abstractions.Data;

public interface ICommandsRepository<TEntity> where TEntity : Entity 
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}