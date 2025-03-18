namespace PaymentsService.Domain.Models;

public record EmployerAccountModel
{
    public string Id { get; init; }
    public string OwnerEmail { get; init; }
    public string Currency { get; init; }
    public long Balance { get; init; }
}