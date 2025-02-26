using System.Linq.Expressions;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Domain.Primitives;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories;

public class AppRepository<TEntity>(IMongoDatabase database, string collectionName) : IRepository<TEntity>
    where TEntity : Entity
{
    protected readonly IMongoCollection<TEntity> _collection = database.GetCollection<TEntity>(collectionName);
    
    public async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default) 
    { 
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken); 
    }
    
    public async Task ReplaceAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity, cancellationToken: cancellationToken);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(e => e.Id == id, cancellationToken);
    }
    
    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        return (int)await _collection.CountDocumentsAsync(FilterDefinition<TEntity>.Empty, cancellationToken: cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
    
    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, 
        CancellationToken cancellationToken = default)
    {
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter, 
        CancellationToken cancellationToken = default)
    {
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAllAsync(int offset, int limit, 
        CancellationToken cancellationToken = default)
    {
        return await _collection.Find(FilterDefinition<TEntity>.Empty)
            .Skip(offset)
            .Limit(limit)
            .ToListAsync(cancellationToken);
    }
}