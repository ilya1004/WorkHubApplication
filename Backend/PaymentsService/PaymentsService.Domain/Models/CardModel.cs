namespace PaymentsService.Domain.Models;

public record CardModel
{
    public string Brand { get; init; }
    public string Country { get; init; }
    public long ExpMonth { get; init; }
    public long ExpYear { get; init; }
    public string Last4Digits { get; init; }
}