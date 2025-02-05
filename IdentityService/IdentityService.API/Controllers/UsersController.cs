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
    public async Task<IActionResult> Login(LoginUserRequest request)
    {

    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {

    }
}
