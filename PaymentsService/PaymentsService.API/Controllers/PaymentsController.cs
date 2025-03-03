using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.SetupPaymentMethod;

namespace PaymentsService.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IMediator mediator) : ControllerBase
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
    public async Task<IActionResult> CreatePaymentByProject([FromRoute] Guid projectId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new PayForProjectWithSavedMethodCommand(projectId), cancellationToken);

        return NoContent();
    }

    [HttpPost("confirm-payment-for-project/{projectId:guid}")]
    public async Task<IActionResult> ConfirmPayment([FromRoute] Guid projectId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new ConfirmPaymentForProjectCommand(projectId), cancellationToken);
        
        return NoContent();
    }
}