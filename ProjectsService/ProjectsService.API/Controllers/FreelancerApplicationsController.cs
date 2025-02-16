using System.Security.Claims;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;

namespace ProjectsService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FreelancerApplicationsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateFreelancerApplication([FromBody] CreateFreelancerApplicationRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateFreelancerApplicationCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!), 
            request.ProjectId), cancellationToken);

        return Created();
    }

    [HttpDelete]
    public async Task<IActionResult> RevokeFreelancerApplication([FromBody] DeleteFreelancerApplicationRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteFreelancerApplicationCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!), 
            request.ProjectId), cancellationToken);

        return NoContent();
    }
}