using ProjectsService.Domain.Primitives;

namespace ProjectsService.Application.Models;

public record PaginatedResultModel<TEntity> where TEntity : Entity
{
    public List<TEntity> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}