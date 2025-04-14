using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;
using IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetAllUsers;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;
using IdentityService.API.Contracts.CommonContracts;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentEmployerUser;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentFreelancerUser;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetEmployerUserInfoById;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetFreelancerUserInfoById;

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

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost]
    [Route("register-employer")]
    public async Task<IActionResult> RegisterEmployer(RegisterEmployerRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<RegisterEmployerCommand>(request), cancellationToken);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllUsers([FromQuery] GetPaginatedListRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllUsersQuery(request.PageNo, request.PageSize), cancellationToken);

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
    [Route("freelancer-info/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetFreelancerUserInfoById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFreelancerUserInfoByIdQuery(userId), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("employer-info/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetEmployerUserInfoById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEmployerUserInfoByIdQuery(userId), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("my-freelancer-info")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> GetCurrentFreelancerUserInfo(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCurrentFreelancerUserQuery(), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("my-employer-info")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> GetCurrentEmployerUserInfo(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCurrentEmployerUserQuery(), cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    [Route("update-freelancer")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> UpdateFreelancerProfile([FromForm] UpdateFreelancerProfileRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<UpdateFreelancerProfileCommand>(request), cancellationToken);

        return NoContent();
    }

    [HttpPut]
    [Route("update-employer")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> UpdateEmployerProfile([FromForm] UpdateEmployerProfileRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<UpdateEmployerProfileCommand>(request), cancellationToken);

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