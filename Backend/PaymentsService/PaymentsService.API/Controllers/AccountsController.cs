using PaymentsService.Applications.UseCases.AccountUseCases.Commands.CreateEmployerAccount;
using PaymentsService.Applications.UseCases.AccountUseCases.Commands.CreateFreelancerAccount;
using PaymentsService.Applications.UseCases.AccountUseCases.Queries.GetEmployerAccount;
using PaymentsService.Applications.UseCases.AccountUseCases.Queries.GetFreelancerAccount;

namespace PaymentsService.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("employer")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> CreateEmployerAccount(CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateEmployerAccountCommand(), cancellationToken);

        return Created();
    }

    [HttpPost]
    [Route("freelancer")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> CreateFreelancerAccount(CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateFreelancerAccountCommand(), cancellationToken);

        return Created();
    }

    [HttpGet]
    [Route("employer/my-account")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> GetEmployerAccount(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetEmployerAccountQuery(), cancellationToken);

        return Ok(result);
    }

    [HttpGet("freelancer/my-account")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> GetFreelancerAccount(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetFreelancerAccountQuery(), cancellationToken);

        return Ok(result);
    }
}