using System.Linq.Expressions;
using ChatService.Domain.Entities;

namespace ChatService.Domain.Abstractions.Repositories;

public interface IChatsRepository
{
    Task InsertAsync(Chat entity, CancellationToken cancellationToken = default);
    Task ReplaceAsync(Chat entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAllAsync(CancellationToken cancellationToken = default);
    Task<Chat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Chat?> FirstOrDefaultAsync(Expression<Func<Chat, bool>> filter, 
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Chat>> PaginatedListAllAsync(int offset, int limit,
        CancellationToken cancellationToken = default);
}