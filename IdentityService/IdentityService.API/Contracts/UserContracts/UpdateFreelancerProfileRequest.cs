using IdentityService.BLL.DTOs;

namespace IdentityService.API.Contracts.UserContracts;

public sealed record UpdateFreelancerProfileRequest(
    FreelancerProfileDTO FreelancerProfile,
    IFormFile? ImageFile);