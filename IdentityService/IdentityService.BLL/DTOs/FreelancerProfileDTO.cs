namespace IdentityService.BLL.DTOs;

public record FreelancerProfileDTO(
    string FirstName,
    string LastName,
    string About,
    IEnumerable<Guid> SkillIds,
    bool ResetImage);