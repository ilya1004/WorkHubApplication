using IdentityService.DAL.Entities;
using IdentityService.DAL.Primitives;
using System.Linq.Expressions;

namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUsersRepository
{
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<AppUser, object>>[]? includesProperties);
    Task UpdateAsync(AppUser entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(AppUser entity, CancellationToken cancellationToken = default);
}
