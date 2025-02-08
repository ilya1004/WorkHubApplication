using IdentityService.BLL.DTOs;
using IdentityService.BLL.Services.TokenProvider;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthTokensDTO>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IConfiguration _configuration;

    public LoginUserCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenProvider tokenService, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenProvider = tokenService;
        _configuration = configuration;
    }

    public async Task<AuthTokensDTO> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedException("You need to confirm your email.");
        }

        var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault("");

        var accessToken = _tokenProvider.GenerateAccessToken(user, userRole);
        var refreshToken = _tokenProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        var expiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"]!);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await _userManager.UpdateAsync(user);

        return new AuthTokensDTO(accessToken, refreshToken);
    }
}
