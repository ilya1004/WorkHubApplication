using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;
using IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetAllUsers;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUsersByRole;
using System.Security.Claims;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Route("register-freelancer")]
    public async Task<IActionResult> RegisterFreelancer(RegisterFreelancerRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<RegisterFreelancerCommand>(request), cancellationToken);

        return Created();
    }

    [HttpPost]
    [Route("register-employer")]
    public async Task<IActionResult> RegisterEmployer(RegisterEmployerRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<RegisterEmployerCommand>(request), cancellationToken);

        return Created();
    }

    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllUsersQuery(pageNo, pageSize), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("by-role")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetUsersByRole([FromQuery] GetUsersByRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(mapper.Map<GetUsersByRoleQuery>(request), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("{userId:guid}")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("my-info")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUserInfo(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!)), cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    [Route("update-freelancer")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> UpdateFreelancerProfile([FromForm] UpdateFreelancerProfileRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateFreelancerProfileCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            request.FreelancerProfile,
            request.ImageFile?.OpenReadStream(),
            request.ImageFile?.ContentType), cancellationToken);

        return NoContent();
    }

    [HttpPut]
    [Route("update-employer")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> UpdateEmployerProfile([FromForm] UpdateEmployerProfileRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateEmployerProfileCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            request.EmployerProfile,
            request.ImageFile?.OpenReadStream(),
            request.ImageFile?.ContentType), cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<ChangePasswordCommand>(request), cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    [Route("{userId:guid}")]
    [Authorize(Policy = AuthPolicies.AdminOrSelfPolicy)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserCommand(userId), cancellationToken);

        return NoContent();
    }
}
