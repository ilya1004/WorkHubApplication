using ChatService.Domain.Abstractions.Repositories;
using ChatService.Infrastructure.Constants;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories;

public class MessagesRepository(IMongoDatabase database) :
    AppRepository<Message>(database, MongoDbCollections.Messages), IMessagesRepository
{
    private readonly IMongoCollection<Message> _messages = database.GetCollection<Message>(MongoDbCollections.Messages);
    
    public async Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await _messages.Find(m => m.ChatId == chatId)
            .SortByDescending(m => m.CreatedAt)
            .Skip(offset)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }
}