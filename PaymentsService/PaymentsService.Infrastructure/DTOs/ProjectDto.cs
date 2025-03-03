namespace PaymentsService.Infrastructure.DTOs;

public record ProjectDto
{
    public int Budget { get; init; }
    public Guid FreelancerId { get; init; }
    public string? PaymentIntentId { get; init; }
}