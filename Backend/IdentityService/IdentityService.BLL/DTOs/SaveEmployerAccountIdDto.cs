namespace IdentityService.BLL.DTOs;

public record SaveEmployerAccountIdDto
{
    public required string UserId { get; init; }
    public required string EmployerAccountId { get; init; }
}