using ProjectsService.Domain.Primitives;

namespace ProjectsService.Domain.Entities;

public class Category : Entity
{
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public ICollection<Project> Projects { get; set; }
}
