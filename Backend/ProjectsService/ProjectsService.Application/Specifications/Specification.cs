using System.Linq.Expressions;
using ProjectsService.Domain.Abstractions.Specification;
using ProjectsService.Domain.Primitives;

namespace ProjectsService.Application.Specifications;

public class Specification<TEntity> : ISpecification<TEntity> where TEntity : Entity
{
    public Expression<Func<TEntity, bool>>? Criteria { get; }
    public List<Expression<Func<TEntity, object>>> IncludesExpression { get; } = [];
    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByDescExpression { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }
    public bool IsPaginationEnabled { get; private set; }

    protected Specification(Expression<Func<TEntity, bool>> criteria)
    {
        Criteria = criteria;
    }

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        IncludesExpression.Add(includeExpression);
    }

    protected void AddPagination(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPaginationEnabled = true;
    }

    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
    {
        OrderByExpression = orderByExpression;
    }

    protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression)
    {
        OrderByDescExpression = orderByDescExpression;
    }
}