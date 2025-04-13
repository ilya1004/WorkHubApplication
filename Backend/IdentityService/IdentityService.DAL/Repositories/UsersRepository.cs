using System.Linq.Expressions;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;

namespace IdentityService.DAL.Repositories;

public class UsersRepository(ApplicationDbContext context) : IUsersRepository
{
    public async Task<AppUser?> GetByIdAsync(Guid id, bool withTracking = true, CancellationToken cancellationToken = default,
        params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        var query = withTracking ? 
            context.AppUsers.AsQueryable() :
            context.AppUsers.AsQueryable().AsNoTracking();

        if (includesProperties != null)
            foreach (var includeProperty in includesProperties)
                query = query.Include(includeProperty);

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<AppUser?> FirstOrDefaultAsync(Expression<Func<AppUser, bool>> filter, CancellationToken cancellationToken = default,
        params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        var query = context.AppUsers.AsQueryable().AsNoTracking();

        if (includesProperties != null)
            foreach (var includeProperty in includesProperties)
                query = query.Include(includeProperty);

        return await query.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<IReadOnlyList<AppUser>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default, 
        params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        var query = context.AppUsers.AsQueryable().AsNoTracking();
        
        if (includesProperties != null)
            foreach (var includeProperty in includesProperties)
                query = query.Include(includeProperty);
        
        return await query
            .OrderBy(x => x.RegisteredAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppUser>> PaginatedListAsync(Expression<Func<AppUser, bool>>? filter, int offset, int limit,
        CancellationToken cancellationToken = default, params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        var query = context.AppUsers.AsQueryable().AsNoTracking();

        if (filter != null) query = query.Where(filter);

        if (includesProperties != null)
            foreach (var includeProperty in includesProperties)
                query = query.Include(includeProperty);

        return await query
            .OrderBy(x => x.RegisteredAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(AppUser entity, CancellationToken cancellationToken = default)
    {
        context.AppUsers.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AppUser entity, CancellationToken cancellationToken = default)
    {
        context.AppUsers.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<AppUser, bool>>? filter, CancellationToken cancellationToken = default)
    {
        var query = context.AppUsers.AsQueryable();

        if (filter != null) query = query.Where(filter);

        return await query.CountAsync(cancellationToken);
    }
}