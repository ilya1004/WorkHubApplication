using System.Linq.Expressions;
using ChatService.Domain.Entities;

namespace ChatService.Domain.Abstractions.Repositories;

public interface IMessagesRepository
{
    Task InsertAsync(Message entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Message>> GetMessagesByChatIdAsync(Guid chatId, int offset, int limit,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Message, bool>> filter, CancellationToken cancellationToken = default);
}