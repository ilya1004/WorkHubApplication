using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IdentityService.DAL.Repository;

public class UsersRepository(ApplicationDbContext context) : IUsersRepository
{
    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        IQueryable<AppUser> query = context.AppUsers.AsQueryable().AsNoTracking();

        if (includesProperties != null)
        {
            foreach (var includeProperty in includesProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AppUser>> PaginatedListAsync(Expression<Func<AppUser, bool>>? filter, int offset, int limit, CancellationToken cancellationToken = default, params Expression<Func<AppUser, object>>[]? includesProperties)
    {
        IQueryable<AppUser> query = context.AppUsers.AsQueryable().AsNoTracking();

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
}
