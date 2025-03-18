namespace PaymentsService.Domain.Models;

public record PaymentMethodModel
{
    public string Id { get; init; }
    public string Type { get; init; }
    public CardModel? Card { get; init; }
    public DateTime CreatedAt { get; init; }
}