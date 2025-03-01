namespace PaymentsService.Domain.DTOs;

public record FreelancerAccountDto
{
    public string Id { get; init; }
    public string OwnerEmail { get; init; }
    public string AccountType { get; init; }
    public IEnumerable<BalanceAmountDto> Balance { get; init; }
}