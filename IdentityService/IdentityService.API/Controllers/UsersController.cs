using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUsersByRole;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UsersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
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

    [HttpGet]
    [Route("by-role")]
    public async Task<IActionResult> GetUsersByRole([FromQuery] GetUsersByRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(_mapper.Map<GetUsersByRoleQuery>(request), cancellationToken);

        return Ok(result);
    }
}
