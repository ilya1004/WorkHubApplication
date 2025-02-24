using ChatService.Domain.Entities;

namespace ChatService.Domain.Abstractions.Repositories;

public interface IMessagesRepository : IRepository<Message>
{
    Task<IReadOnlyList<Message>> GetMessagesByChatIdAsync(Guid chatId, int offset, int limit,
        CancellationToken cancellationToken = default);
}