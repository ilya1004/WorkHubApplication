using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentEmployerUser;

public sealed record GetCurrentEmployerUserQuery : IRequest<EmployerUserDto>;