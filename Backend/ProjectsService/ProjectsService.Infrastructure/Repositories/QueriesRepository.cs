using System.Linq.Expressions;
using ProjectsService.Application.Specifications;
using ProjectsService.Domain.Abstractions.Specification;
using ProjectsService.Domain.Primitives;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Extensions;

namespace ProjectsService.Infrastructure.Repositories;

public class QueriesRepository<TEntity>(QueriesDbContext context) : IQueriesRepository<TEntity> where TEntity : Entity
{
    private readonly DbSet<TEntity> _entities = context.Set<TEntity>();
    
    public async Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default, 
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        var query = _entities.AsQueryable().AsNoTracking();

        query = query.AddIncludes(includesProperties);

        return await query
            .OrderBy(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? filter, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        var query = _entities.AsQueryable().AsNoTracking();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        query = query.AddIncludes(includesProperties);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> PaginatedListAsync(Expression<Func<TEntity, bool>>? filter, int offset, int limit, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        var query = _entities.AsQueryable().AsNoTracking();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        query = query.AddIncludes(includesProperties);

        return await query
            .OrderBy(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[]? includesProperties)
    {
        var query = _entities.AsQueryable().AsNoTracking();

        query = query.AddIncludes(includesProperties);

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

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _entities.CountAsync(filter, cancellationToken);
    }

    public async Task<int> CountByFilterAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator<TEntity>.ToCountQuery(_entities.AsQueryable(), specification);
        
        return await query.CountAsync(cancellationToken);
    }
    
    public async Task<IReadOnlyList<TEntity>> GetByFilterAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator<TEntity>.ToGetQuery(_entities.AsQueryable(), specification);
        
        return await query.ToListAsync(cancellationToken);
    }
}