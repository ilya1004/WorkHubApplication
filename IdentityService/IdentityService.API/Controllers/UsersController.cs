using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;
using IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUsersByRole;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Route("register-freelancer")]
    public async Task<IActionResult> RegisterFreelancer(RegisterFreelancerRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<RegisterFreelancerCommand>(request), cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("register-employer")]
    public async Task<IActionResult> RegisterEmployer(RegisterEmployerRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<RegisterEmployerCommand>(request), cancellationToken);
        
        return Ok();
    }

    [HttpGet]
    [Route("by-role")]
    public async Task<IActionResult> GetUsersByRole([FromQuery] GetUsersByRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<GetUsersByRoleQuery>(request), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("{userId:guid}")]
    public async Task<IActionResult> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    [Route("update-freelancer")]
    public async Task<IActionResult> UpdateFreelancerProfile([FromForm] UpdateFreelancerProfileRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateFreelancerProfileCommand(
            //Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Guid.Parse("0194e7e1-3a39-7dc8-ad84-b3a22400751b"),
            request.FreelancerProfile, 
            request.ImageFile?.OpenReadStream(), 
            request.ImageFile?.ContentType), cancellationToken);
        
        return Ok();
    }

    [HttpPut]
    [Route("update-employer")]
    public async Task<IActionResult> UpdateEmployerProfile([FromForm] UpdateEmployerProfileRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateEmployerProfileCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            request.EmployerProfile,
            request.ImageFile?.OpenReadStream(),
            request.ImageFile?.ContentType), cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<ChangePasswordCommand>(request), cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{userId:guid}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserCommand(userId), cancellationToken);
        return NoContent();
    }
}
