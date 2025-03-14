using IdentityService.BLL.DTOs;

namespace IdentityService.API.Contracts.UserContracts;

public sealed record UpdateEmployerProfileRequest(
    EmployerProfileDto EmployerProfile,
    IFormFile? ImageFile);