using IdentityService.BLL.Abstractions.TokenProvider;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.Settings;

namespace IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

public class LoginUserCommandHandler(
    SignInManager<AppUser> signInManager,
    IUnitOfWork unitOfWork,
    ITokenProvider tokenService,
    IOptions<JwtSettings> options,
    ILogger<LoginUserCommandHandler> logger) : IRequestHandler<LoginUserCommand, AuthTokensDto>
{
    public async Task<AuthTokensDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Login attempt for email {Email}", request.Email);

        var user = await unitOfWork.UsersRepository.FirstOrDefaultAsync(
            u => u.Email == request.Email, 
            cancellationToken, 
            u => u.Role);

        if (user is null)
        {
            logger.LogWarning("Invalid login attempt for email {Email}", request.Email);
            
            throw new UnauthorizedException("Invalid credentials.");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            logger.LogWarning("Invalid password for user {UserId}", user.Id);
            
            throw new UnauthorizedException("Invalid credentials.");
        }

        if (!user.EmailConfirmed)
        {
            logger.LogWarning("Login attempt for unconfirmed email {Email}", request.Email);
            
            throw new UnauthorizedException("You need to confirm your email.");
        }

        logger.LogInformation("Generating tokens for user {UserId}", user.Id);
        
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        var expiryDays = options.Value.RefreshTokenExpiryDays;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await unitOfWork.UsersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Successful login for user {UserId}", user.Id);
        
        return new AuthTokensDto(accessToken, refreshToken);
    }
}