namespace PaymentsService.Domain.Models;

public record ChargeModel
{
    public string Id { get; init; }
    public long Amount { get; init; }
    public string Currency { get; init; }
    public bool Captured { get; init; }
    public string Status { get; init; }
    public string PaymentMethod { get; init; }
}