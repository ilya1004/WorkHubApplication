using System.Linq.Expressions;
using ProjectsService.Domain.Primitives;

namespace ProjectsService.Domain.Abstractions.Specification;

public interface ISpecification<TEntity> where TEntity : Entity
{
    Expression<Func<TEntity, bool>>? Criteria { get; }
    List<Expression<Func<TEntity, object>>> IncludesExpression { get; }
    Expression<Func<TEntity, object>>? OrderByExpression { get; }
    Expression<Func<TEntity, object>>? OrderByDescExpression { get; }
    int? Take { get; }
    int? Skip { get; }
    bool IsPaginationEnabled { get; }   
}