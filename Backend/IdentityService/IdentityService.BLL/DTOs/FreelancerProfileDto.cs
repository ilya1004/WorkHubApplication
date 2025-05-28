namespace IdentityService.BLL.DTOs;

public record FreelancerProfileDto(
    string FirstName,
    string LastName,
    string? About,
    IEnumerable<Guid>? SkillIds,
    bool ResetImage);