using PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.SetupPaymentMethod;
using PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetMyPaymentMethods;

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

    [HttpGet]
    [Route("my-payment-methods")]
    public async Task<IActionResult> GetMyPaymentMethods(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetMyPaymentMethodsQuery(), cancellationToken);

        return Ok(result);
    }
}