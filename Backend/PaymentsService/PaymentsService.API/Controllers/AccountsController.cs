using PaymentsService.API.Contracts.CommonContracts;
using PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateEmployerAccount;
using PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateFreelancerAccount;
using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllEmployerAccounts;
using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllFreelancerAccounts;
using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetEmployerAccount;
using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetFreelancerAccount;

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

    [HttpGet]
    [Route("freelancer/my-account")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> GetFreelancerAccount(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetFreelancerAccountQuery(), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("by-employer")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllEmployerAccount([FromQuery] GetPaginatedListRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllEmployerAccountsQuery(request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("by-freelancer")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllFreelancersAccount([FromQuery] GetPaginatedListRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllFreelancerAccountsQuery(request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }
}