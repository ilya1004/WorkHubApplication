using ProjectsService.Domain.Enums;
using ProjectsService.Domain.Primitives;

namespace ProjectsService.Domain.Entities;

public class FreelancerApplication : Entity
{
    public DateTime CreatedAt { get; set; }
    public ApplicationStatus Status { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }
    public Guid FreelancerId { get; set; }
}
