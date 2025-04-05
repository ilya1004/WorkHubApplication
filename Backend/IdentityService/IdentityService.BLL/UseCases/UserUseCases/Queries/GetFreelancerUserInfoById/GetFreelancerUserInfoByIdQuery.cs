using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetFreelancerUserInfoById;

public sealed record GetFreelancerUserInfoByIdQuery(Guid Id) : IRequest<FreelancerUserDto>;