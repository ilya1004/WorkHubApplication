using IdentityService.BLL.DTOs;
using IdentityService.BLL.Settings;
using System.Security.Claims;
using IdentityService.BLL.Abstractions.TokenProvider;

namespace IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;

public class RefreshTokenCommandHandler(
    ITokenProvider tokenProvider,
    IUnitOfWork unitOfWork,
    IOptions<JwtSettings> options,
    ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
{
    public async Task<AuthTokensDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing refresh token request");

        var principal = tokenProvider.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        logger.LogInformation("Refreshing tokens for user {UserId}", userId);
        
        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            Guid.Parse(userId!),
            false,
            cancellationToken,
            u => u.Role);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            logger.LogWarning("Invalid refresh token for user {UserId}", userId);
            
            throw new UnauthorizedException("Invalid refresh token");
        }

        var newAccessToken = tokenProvider.GenerateAccessToken(user);
        var newRefreshToken = tokenProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        var expiryDays = options.Value.RefreshTokenExpiryDays;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await unitOfWork.UsersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Tokens refreshed successfully for user {UserId}", user.Id);
        
        return new AuthTokensDto(newAccessToken, newRefreshToken);
    }
}