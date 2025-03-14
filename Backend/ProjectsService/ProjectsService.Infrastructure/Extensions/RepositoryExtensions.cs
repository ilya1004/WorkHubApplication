using System.Linq.Expressions;
using ProjectsService.Domain.Primitives;

namespace ProjectsService.Infrastructure.Extensions;

public static class RepositoryExtensions
{
    public static IQueryable<TEntity> AddIncludes<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, object>>[]? includesProperties) where TEntity : Entity
    {
        if (includesProperties is not null)
        {
            foreach (var includeProperty in includesProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return query;
    }
}