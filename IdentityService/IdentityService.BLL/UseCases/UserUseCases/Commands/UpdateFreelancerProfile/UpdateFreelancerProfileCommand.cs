using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;

public sealed record UpdateFreelancerProfileCommand(
    Guid Id,
    FreelancerProfileDTO FreelancerProfile,
    Stream? FileStream,
    string? ContentType) : IRequest;