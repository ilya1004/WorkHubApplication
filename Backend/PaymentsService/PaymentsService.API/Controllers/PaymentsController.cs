using PaymentsService.API.Contracts.CommonContracts;
using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;
using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllEmployerPayments;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllFreelancerTransfers;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentIntents;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerMyTransfers;

namespace PaymentsService.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(
    IMediator mediator,
    IMapper mapper) : ControllerBase
{
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
    public async Task<IActionResult> GetEmployerMyPayments([FromQuery] GetOperationsRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetEmployerMyPaymentsQuery>(request), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("employer-payments")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllEmployerPayments([FromQuery] GetPaginatedListRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllEmployerPaymentsQuery(request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("employer/my-payment-intents")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> GetEmployerMyPaymentIntents([FromQuery] GetOperationsRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetEmployerPaymentIntentsQuery>(request), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("freelancer/my-transfers")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> GetFreelancerMyTransfers([FromQuery] GetOperationsRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetFreelancerMyTransfersQuery>(request), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("freelancer-transfers")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllFreelancerTransfers([FromQuery] GetPaginatedListRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllFreelancerTransfersQuery(request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }
}