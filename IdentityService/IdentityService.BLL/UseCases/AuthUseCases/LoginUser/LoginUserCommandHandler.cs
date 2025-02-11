using IdentityService.BLL.DTOs;
using IdentityService.BLL.Services.TokenProvider;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

public class LoginUserCommandHandler(
    SignInManager<AppUser> signInManager,
    IUnitOfWork unitOfWork,
    ITokenProvider tokenService, 
    IConfiguration configuration) : IRequestHandler<LoginUserCommand, AuthTokensDTO>
{
    public async Task<AuthTokensDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UsersRepository.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken, u => u.Role);

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

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        var expiryDays = int.Parse(configuration["Jwt:RefreshTokenExpiryDays"]!);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await unitOfWork.UsersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        return new AuthTokensDTO(accessToken, refreshToken);
    }
}
