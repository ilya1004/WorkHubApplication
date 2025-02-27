using System.Linq.Expressions;

namespace ChatService.Domain.Abstractions.Repositories;

// public interface IRepository<TEntity>
// {
//     Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
//     Task ReplaceAsync(TEntity entity, CancellationToken cancellationToken = default);
//     Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
//     Task<int> CountAllAsync(CancellationToken cancellationToken = default);
//     Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
//     Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
//     Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, 
//         CancellationToken cancellationToken = default);
//     Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter, 
//         CancellationToken cancellationToken = default);
//     Task<IReadOnlyList<TEntity>> PaginatedListAllAsync(int offset, int limit,
//         CancellationToken cancellationToken = default);
// }