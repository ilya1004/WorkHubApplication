using IdentityService.DAL.Abstractions.Data;
using IdentityService.DAL.Data;
using IdentityService.DAL.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IdentityService.DAL.Repository;

public class AppRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _entities;

    public AppRepository(ApplicationDbContext context)
    {
        _context = context;
        _entities = context.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);

        return Task.CompletedTask;
    }
    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking().FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        IQueryable<TEntity> query = _entities.AsQueryable().AsNoTracking();

        if (includesProperties != null)
        {
            foreach (var includeProperty in includesProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

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

    public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? filter, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        IQueryable<TEntity> query = _entities.AsQueryable().AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includesProperties != null)
        {
            foreach (var includeProperty in includesProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAsync(Expression<Func<TEntity, bool>>? filter, int offset, int limit, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        IQueryable<TEntity> query = _entities.AsQueryable().AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includesProperties != null)
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

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _entities.Update(entity);

        return Task.CompletedTask;
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _entities.AnyAsync(filter, cancellationToken);
    }
}
