namespace IdentityService.BLL.DTOs;

public record EmployerProfileDto(
    string CompanyName,
    string? About,
    Guid? IndustryId,
    bool ResetImage);