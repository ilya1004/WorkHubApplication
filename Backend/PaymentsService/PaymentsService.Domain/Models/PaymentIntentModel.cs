namespace PaymentsService.Domain.Models;

public record PaymentIntentModel
{
    public string Id { get; init; }
    public long Amount { get; init; }
    public string Currency { get; init; }
    public string Status { get; init; }
    public DateTime Created { get; init; }
    public string TransferGroup { get; init; }
    public Dictionary<string, string> Metadata { get; init; }
}