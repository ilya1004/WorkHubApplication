namespace PaymentsService.Domain.Models;

public record FreelancerAccountModel
{
    public string Id { get; init; }
    public string OwnerEmail { get; init; }
    public string AccountType { get; init; }
    public string Country { get; init; }
    public long Balance { get; init; }
}