using IdentityService.BLL.DTOs;
using IdentityService.BLL.Services.TokenProvider;
using IdentityService.BLL.Settings;
using IdentityService.DAL.Abstractions.Repositories;
using Microsoft.Extensions.Options;

namespace IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

public class LoginUserCommandHandler(
    SignInManager<AppUser> signInManager,
    IUnitOfWork unitOfWork,
    ITokenProvider tokenService,
    IOptions<JwtSettings> options) : IRequestHandler<LoginUserCommand, AuthTokensDto>
{
    public async Task<AuthTokensDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UsersRepository.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken, u => u.Role);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedException("You need to confirm your email.");
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        var expiryDays = options.Value.RefreshTokenExpiryDays;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await unitOfWork.UsersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        return new AuthTokensDto(accessToken, refreshToken);
    }
}
