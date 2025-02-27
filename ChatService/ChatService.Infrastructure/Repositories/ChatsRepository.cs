using ChatService.Domain.Abstractions.Repositories;
using ChatService.Infrastructure.Constants;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories;

public class ChatsRepository(IMongoDatabase database) : IChatsRepository
{
    private readonly IMongoCollection<Chat> _collection = 
        database.GetCollection<Chat>(MongoDbCollections.Chats);
    
    public async Task InsertAsync(Chat entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken); 
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        return (int)await _collection.CountDocumentsAsync(FilterDefinition<Chat>.Empty, cancellationToken: cancellationToken);
    }

    public async Task<Chat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Chat>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(FilterDefinition<Chat>.Empty)
            .Skip(offset)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }
}