using AutoMapper;
using IdentityService.API.Contracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

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
    public async Task<IActionResult> RegisterFreelancer(RegisterFreelancerRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(_mapper.Map<RegisterFreelancerCommand>(request), cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("register-employer")]
    public async Task<IActionResult> RegisterEmployer(RegisterEmployerRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(_mapper.Map<RegisterEmployerCommand>(request), cancellationToken);

        return Ok();
    }
}
