using ProjectsService.Domain.Enums;
using ProjectsService.Domain.Primitives;

namespace ProjectsService.Domain.Entities;

public class Lifecycle : Entity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ApplicationsStartDate { get; set; }
    public DateTime ApplicationsDeadline { get; set; }
    public DateTime WorkStartDate { get; set; }
    public DateTime WorkDeadline { get; set; }
    public ProjectStatus Status { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }
}
