using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthTokensDto>;