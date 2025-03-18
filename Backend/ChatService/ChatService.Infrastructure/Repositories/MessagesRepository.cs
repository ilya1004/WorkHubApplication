using System.Linq.Expressions;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Infrastructure.Constants;

namespace ChatService.Infrastructure.Repositories;

public class MessagesRepository(IMongoDatabase database) : IMessagesRepository
{
    private readonly IMongoCollection<Message> _collection = 
        database.GetCollection<Message>(MongoDbCollections.Messages);
    
    public async Task InsertAsync(Message entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken); 
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Message>> GetMessagesByChatIdAsync(Guid chatId, int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(m => m.ChatId == chatId)
            .SortByDescending(m => m.CreatedAt)
            .Skip(offset)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Message, bool>> filter, CancellationToken cancellationToken = default)
    {
        return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}