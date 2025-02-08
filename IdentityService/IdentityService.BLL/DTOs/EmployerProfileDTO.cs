namespace IdentityService.BLL.DTOs;

public record EmployerProfileDTO(
    string CompanyName,
    string About,
    Guid? IndustryId);
