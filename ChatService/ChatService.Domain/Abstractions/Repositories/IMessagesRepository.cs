using ChatService.Domain.Entities;

namespace ChatService.Domain.Abstractions.Repositories;

public interface IMessagesRepository : IRepository<Message>
{
    Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, int offset, int limit,
        CancellationToken cancellationToken = default);
}