using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.SavePaymentMethod;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.SetupPaymentMethod;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetMyPaymentMethods;

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
    [Route("save-payment-method/{paymentMethodId:guid}")]
    public async Task<IActionResult> SavePaymentMethod(Guid paymentMethodId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new SavePaymentMethodCommand(paymentMethodId), cancellationToken);

        return NoContent();
    }

    [HttpGet]
    [Route("my-payment-methods")]
    public async Task<IActionResult> GetMyPaymentMethods(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetMyPaymentMethodsQuery(), cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Route("pay-for-project/{projectId:guid}/with-saved-method")]
    public async Task<IActionResult> CreatePaymentByProject(Guid projectId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new PayForProjectWithSavedMethodCommand(projectId), cancellationToken);

        return NoContent();
    }
}