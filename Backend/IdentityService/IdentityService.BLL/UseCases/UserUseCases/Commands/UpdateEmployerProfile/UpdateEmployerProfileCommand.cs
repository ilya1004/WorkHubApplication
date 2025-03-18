using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;

public sealed record UpdateEmployerProfileCommand(
    EmployerProfileDto EmployerProfile,
    Stream? FileStream,
    string? ContentType) : IRequest;