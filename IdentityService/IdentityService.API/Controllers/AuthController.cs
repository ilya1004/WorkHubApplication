using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;
using IdentityService.BLL.UseCases.AuthUseCases.LoginUser;
using IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

        return Ok();
    }

    
}
