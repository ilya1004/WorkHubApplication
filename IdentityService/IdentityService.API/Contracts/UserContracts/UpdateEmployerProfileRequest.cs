using IdentityService.BLL.DTOs;

namespace IdentityService.API.Contracts.UserContracts;

public sealed record UpdateEmployerProfileRequest(
    Guid Id,
    EmployerProfileDTO EmployerProfile,
    IFormFile? ImageFile);
