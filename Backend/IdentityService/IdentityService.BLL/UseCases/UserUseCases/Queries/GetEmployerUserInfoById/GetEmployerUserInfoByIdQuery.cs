using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetEmployerUserInfoById;

public sealed record GetEmployerUserInfoByIdQuery(Guid Id) : IRequest<EmployerUserDto>;