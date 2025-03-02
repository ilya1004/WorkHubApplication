using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.SetupPaymentMethod;

namespace PaymentsService.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("setup-payment-method")]
    public async Task<IActionResult> SetupPaymentMethod(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new SetupPaymentMethodCommand(), cancellationToken);
        
        return Ok(result);
    }
}