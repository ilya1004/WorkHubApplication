using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;
using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.CreateSetupIntent;
using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentsQuery;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerTransfers;

namespace PaymentsService.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(
    IMediator mediator,
    IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Route("setup-intent")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> CreateSetupIntent(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new CreateSetupIntentCommand(), cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("pay-for-project/{projectId:guid}/with-method/{paymentMethodId}")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> CreatePaymentByProject([FromRoute] Guid projectId, [FromRoute] string paymentMethodId,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new PayForProjectWithSavedMethodCommand(projectId, paymentMethodId), cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("confirm-payment-for-project/{projectId:guid}")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> ConfirmPayment([FromRoute] Guid projectId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new ConfirmPaymentForProjectCommand(projectId), cancellationToken);

        return NoContent();
    }

    [HttpGet]
    [Route("employer/my-payments")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> GetEmployerPayments([FromQuery] GetOperationsRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetEmployerPaymentsQuery>(request), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("freelancer/my-transfers")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> GetFreelancerTransfers([FromQuery] GetOperationsRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetFreelancerTransferQuery>(request), cancellationToken);

        return Ok(result);
    }
}