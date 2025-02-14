using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Domain.Primitives;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class QueriesRepository<TEntity>(QueriesDbContext context) : IQueriesRepository<TEntity> where TEntity : Entity
{
    protected readonly QueriesDbContext _context = context;
    protected readonly DbSet<TEntity> _entities = context.Set<TEntity>();
    
    public async Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await _entities
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? filter, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        IQueryable<TEntity> query = _entities.AsQueryable().AsNoTracking();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (includesProperties is not null)
        {
            foreach (var includeProperty in includesProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAsync(Expression<Func<TEntity, bool>>? filter, int offset, int limit, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        IQueryable<TEntity> query = _entities.AsQueryable().AsNoTracking();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (includesProperties is not null)
        {
            foreach (var includeProperty in includesProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query
            .OrderBy(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        IQueryable<TEntity> query = _entities.AsQueryable().AsNoTracking();

        if (includesProperties is not null)
        {
            foreach (var includeProperty in includesProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking().FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _entities.AnyAsync(filter, cancellationToken);
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        return await _entities.CountAsync(cancellationToken);
    }
}