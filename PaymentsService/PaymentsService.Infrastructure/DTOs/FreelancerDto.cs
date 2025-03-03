namespace PaymentsService.Infrastructure.DTOs;

public record FreelancerDto
{
    public string? StripeAccountId { get; init; }
};