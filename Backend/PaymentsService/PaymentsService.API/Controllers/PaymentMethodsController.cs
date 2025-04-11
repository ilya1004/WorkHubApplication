using PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.DeletePaymentMethod;
using PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.SavePaymentMethod;
using PaymentsService.Application.UseCases.PaymentMethodUseCases.Queries.GetMyPaymentMethods;

namespace PaymentsService.API.Controllers;

[ApiController]
[Route("api/payment-methods")]
public class PaymentMethodsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("{paymentMethodId}")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> SavePaymentMethod([FromRoute] string paymentMethodId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new SavePaymentMethodCommand(paymentMethodId), cancellationToken);

        return NoContent();
    }

    [HttpGet]
    [Route("my-payment-methods")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> GetMyPaymentMethods(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetMyPaymentMethodsQuery(), cancellationToken);

        return Ok(result);
    }

    [HttpDelete]
    [Route("{paymentMethodId}")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> DeletePaymentMethod([FromRoute] string paymentMethodId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeletePaymentMethodCommand(paymentMethodId), cancellationToken);

        return NoContent();
    }
}