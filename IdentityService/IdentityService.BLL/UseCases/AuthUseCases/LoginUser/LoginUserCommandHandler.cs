using IdentityService.BLL.DTOs;
using IdentityService.BLL.Services.TokenProvider;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

public class LoginUserCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenProvider tokenService, IConfiguration configuration) : IRequestHandler<LoginUserCommand, AuthTokensDTO>
{
    public async Task<AuthTokensDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
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

        var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault("");

        var accessToken = tokenService.GenerateAccessToken(user, userRole);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        var expiryDays = int.Parse(configuration["Jwt:RefreshTokenExpiryDays"]!);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await userManager.UpdateAsync(user);

        return new AuthTokensDTO(accessToken, refreshToken);
    }
}
