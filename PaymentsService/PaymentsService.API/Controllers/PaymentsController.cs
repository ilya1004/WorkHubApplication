using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.SetupPaymentMethod;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentsQuery;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetFreelancerTransfers;

namespace PaymentsService.API.Controllers;


[ApiController]
[Route("api/payments")]
public class PaymentsController(
    IMediator mediator,
    IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Route("create-setup-intent")]
    public async Task<IActionResult> CreateSetupIntent(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new CreateSetupIntentCommand(), cancellationToken);
        
        return Ok(result);
    }

    [HttpPost]
    [Route("pay-for-project/{projectId:guid}/with-saved-method")]
    public async Task<IActionResult> CreatePaymentByProject([FromRoute] Guid projectId, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new PayForProjectWithSavedMethodCommand(projectId), cancellationToken);

        return NoContent();
    }

    [HttpPost("confirm-payment-for-project/{projectId:guid}")]
    public async Task<IActionResult> ConfirmPayment([FromRoute] Guid projectId, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new ConfirmPaymentForProjectCommand(projectId), cancellationToken);
        
        return NoContent();
    }

    [HttpGet]
    [Route("employer/my-payments")]
    public async Task<IActionResult> GetEmployerPayments([FromQuery] GetOperationsRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetEmployerPaymentsQuery>(request), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("freelancer/my-transfers")]
    public async Task<IActionResult> GetFreelancerTransfers([FromQuery] GetOperationsRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetFreelancerTransferQuery>(request), cancellationToken);

        return Ok(result);
    }
}