using IdentityService.BLL.DTOs;

namespace IdentityService.API.Contracts.UserContracts;

public sealed record UpdateEmployerProfileRequest(
    EmployerProfileDTO EmployerProfile,
    IFormFile? ImageFile);
