using ChatService.Domain.Primitives;

namespace ChatService.Domain.Entities;

public class Chat : Entity
{
    public Guid EmployerId { get; set; }
    public Guid FreelancerId { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}