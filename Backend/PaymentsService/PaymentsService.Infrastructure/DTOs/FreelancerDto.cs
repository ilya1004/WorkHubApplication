namespace PaymentsService.Infrastructure.DTOs;

public record FreelancerDto
{
    public string Id { get; init; }
    public string? StripeAccountId { get; init; }
}