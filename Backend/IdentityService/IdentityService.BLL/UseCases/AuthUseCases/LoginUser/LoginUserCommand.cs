using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<AuthTokensDto>;