using IdentityService.API.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        return Ok();
    }

    [HttpPost]
    [Route("register-freelancer")]
    public async Task<IActionResult> Register(RegisterFreelancerRequest request)
    {
        return Ok();
    }

    [HttpPost]
    [Route("register-employer")]
    public async Task<IActionResult> RegisterEmployer(RegisterEmployerRequest request)
    {
        return Ok();
    }
}
