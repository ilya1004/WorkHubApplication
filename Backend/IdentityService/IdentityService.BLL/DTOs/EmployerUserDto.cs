namespace IdentityService.BLL.DTOs;

public record EmployerUserDto
{
    public string Id { get; init; }
    public string UserName { get; init; }
    public string CompanyName { get; init; }
    public string About { get; init; }
    public string Email { get; init; }
    public DateTime RegisteredAt { get; init; }
    public string? StripeCustomerId { get; set; }
    public EmployerIndustryDto? Industry { get; init; }
    public string? ImageUrl { get; init; }
    public string RoleName { get; init; }
}