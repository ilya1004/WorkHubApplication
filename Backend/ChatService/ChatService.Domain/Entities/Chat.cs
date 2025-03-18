namespace ChatService.Domain.Entities;

public class Chat
{
    public Guid Id { get; set; }
    public Guid EmployerId { get; set; }
    public Guid FreelancerId { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}