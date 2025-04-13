using System.Linq.Expressions;

namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUsersRepository
{
    Task<AppUser?> GetByIdAsync(Guid id, bool withTracking = true, CancellationToken cancellationToken = default,
        params Expression<Func<AppUser, object>>[]? includesProperties);

    Task<AppUser?> FirstOrDefaultAsync(Expression<Func<AppUser, bool>> filter, CancellationToken cancellationToken = default,
        params Expression<Func<AppUser, object>>[]? includesProperties);

    Task<IReadOnlyList<AppUser>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default, 
        params Expression<Func<AppUser, object>>[]? includesProperties);

    Task<IReadOnlyList<AppUser>> PaginatedListAsync(Expression<Func<AppUser, bool>>? filter, int offset, int limit,
        CancellationToken cancellationToken = default, params Expression<Func<AppUser, object>>[]? includesProperties);

    Task UpdateAsync(AppUser entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(AppUser entity, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<AppUser, bool>>? filter, CancellationToken cancellationToken = default);
}