namespace PaymentsService.Domain.DTOs;

public record PaymentMethodDto
{
    public string Type { get; init; }
    public CardDto? Card { get; init; }
    public DateTime CreatedAt { get; init; }
}