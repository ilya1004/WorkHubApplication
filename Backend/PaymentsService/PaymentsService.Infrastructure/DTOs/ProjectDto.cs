namespace PaymentsService.Infrastructure.DTOs;

public record ProjectDto
{
    public Guid Id { get; init; }
    public int BudgetInCents { get; init; }
    public Guid? FreelancerId { get; init; }
    public string? PaymentIntentId { get; init; }
}