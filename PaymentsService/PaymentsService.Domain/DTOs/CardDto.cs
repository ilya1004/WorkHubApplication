namespace PaymentsService.Domain.DTOs;

public record CardDto
{
    public string Brand { get; init; }
    public string CardholderName { get; init; }
    public string Country { get; init; }
    public long ExpMonth { get; init; }
    public long ExpYear { get; init; }
    public string Last4Digits { get; init; } 
}