using IdentityService.BLL.DTOs;
using IdentityService.BLL.Services.TokenProvider;
using IdentityService.BLL.Settings;
using IdentityService.DAL.Abstractions.Repositories;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;

public class RefreshTokenCommandHandler(
    ITokenProvider tokenProvider,
    IUnitOfWork unitOfWork,
    IOptions<JwtSettings> options) : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
{
    public async Task<AuthTokensDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = tokenProvider.GetPrincipalFromExpiredToken(request.AccessToken);

        var user = await unitOfWork.UsersRepository.GetByIdAsync(Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!),
            cancellationToken, u => u.Role);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new UnauthorizedException("Invalid refresh token");

        var newAccessToken = tokenProvider.GenerateAccessToken(user);
        var newRefreshToken = tokenProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        var expiryDays = options.Value.RefreshTokenExpiryDays;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await unitOfWork.UsersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        return new AuthTokensDto(newAccessToken, newRefreshToken);
    }
}