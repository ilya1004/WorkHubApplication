namespace PaymentsService.Domain.Models;

public record PaymentIntentModel
{
    public long Amount { get; init; }
    public string Currency { get; init; }
    public string TransferGroup { get; init; }
}