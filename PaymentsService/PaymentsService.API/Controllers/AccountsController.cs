using PaymentsService.Applications.UseCases.AccountUseCases.Queries.GetEmployerAccount;
using PaymentsService.Applications.UseCases.AccountUseCases.Queries.GetFreelancerAccount;

namespace PaymentsService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Route("employer")]
    public async Task<IActionResult> GetEmployerAccount(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetEmployerAccountQuery(), cancellationToken);

        return Ok(result);
    }

    [HttpGet("freelancer")]
    public async Task<IActionResult> GetFreelancerAccount(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetFreelancerAccountQuery(), cancellationToken);

        return Ok(result);
    }
}