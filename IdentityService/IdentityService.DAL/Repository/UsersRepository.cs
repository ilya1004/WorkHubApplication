using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IdentityService.DAL.Repository;

public class UsersRepository(ApplicationDbContext context)
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
}
