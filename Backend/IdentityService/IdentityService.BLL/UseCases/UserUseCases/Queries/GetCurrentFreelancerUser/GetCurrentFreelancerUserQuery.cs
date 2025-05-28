using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentFreelancerUser;

public sealed record GetCurrentFreelancerUserQuery : IRequest<FreelancerUserDto>;