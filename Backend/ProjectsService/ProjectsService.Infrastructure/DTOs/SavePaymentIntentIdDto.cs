namespace ProjectsService.Infrastructure.DTOs;

public record SavePaymentIntentIdDto
{
    public required string ProjectId { get; init; }
    public required string PaymentIntentId { get; init; }
}