using IdentityService.BLL.DTOs;

namespace IdentityService.API.Contracts.UserContracts;

public sealed record UpdateFreelancerProfileRequest(
    FreelancerProfileDto FreelancerProfile,
    IFormFile? ImageFile);