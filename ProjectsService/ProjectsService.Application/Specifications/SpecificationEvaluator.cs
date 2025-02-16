using Microsoft.EntityFrameworkCore;
using ProjectsService.Domain.Abstractions.Specification;
using ProjectsService.Domain.Primitives;

namespace ProjectsService.Application.Specifications;

public static class SpecificationEvaluator<TEntity> where TEntity : Entity
{
    public static IQueryable<TEntity> ToGetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> specification)
    {
        var query = inputQuery;

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.IncludesExpression.Aggregate(query,
            (current, include) => current.Include(include));

        if (specification.OrderByExpression is not null)
        {
            query = query.OrderBy(specification.OrderByExpression);
        }
        else if (specification.OrderByDescExpression is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescExpression);
        }

        if (specification.IsPaginationEnabled)
        {
            if (specification.Skip.HasValue)
            {
                query = query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = query.Take(specification.Take.Value);
            }
        }

        return query;
    }
    
    public static IQueryable<TEntity> ToCountQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> specification)
    {
        var query = inputQuery;

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }
        
        return query;
    }
}