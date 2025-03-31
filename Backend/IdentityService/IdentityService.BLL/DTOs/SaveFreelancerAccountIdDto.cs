namespace IdentityService.BLL.DTOs;

public record SaveFreelancerAccountIdDto
{
    public required string UserId { get; init; }
    public required string FreelancerAccountId { get; init; }
}