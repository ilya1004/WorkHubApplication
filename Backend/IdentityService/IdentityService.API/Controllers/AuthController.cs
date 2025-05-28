using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;
using IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;
using IdentityService.BLL.UseCases.AuthUseCases.LoginUser;
using IdentityService.BLL.UseCases.AuthUseCases.LogoutUser;
using IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;
using IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;
using IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var authResponse = await mediator.Send(mapper.Map<LoginUserCommand>(request), cancellationToken);

        return Ok(authResponse);
    }

    [HttpPost]
    [Route("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await mediator.Send(new LogoutUserCommand(), cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResponse = await mediator.Send(mapper.Map<RefreshTokenCommand>(request), cancellationToken);

        return Ok(authResponse);
    }

    [HttpPost]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<ConfirmEmailCommand>(request), cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("resend-email-confirmation")]
    public async Task<IActionResult> ResendConfirmationEmail(ResendEmailConfirmationRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<ResendEmailConfirmationCommand>(request), cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<ForgotPasswordCommand>(request), cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<ResetPasswordCommand>(request), cancellationToken);

        return NoContent();
    }
}