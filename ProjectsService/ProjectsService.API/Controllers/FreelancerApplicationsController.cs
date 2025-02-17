using System.Security.Claims;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.AcceptFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.RejectFreelancerApplication;

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
    
    
    [HttpPut]
    [Route("{applicationId:guid}/accept-application/{projectId:guid}")]
    public async Task<IActionResult> AcceptApplication([FromRoute] Guid applicationId, [FromRoute] Guid projectId, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new AcceptFreelancerApplicationCommand(
                Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!), 
                projectId,
                applicationId), 
                cancellationToken);

        return NoContent();
    }
    
    [HttpPut]
    [Route("{applicationId:guid}/reject-application/{projectId:guid}")]
    public async Task<IActionResult> RejectApplication([FromRoute] Guid applicationId, [FromRoute] Guid projectId, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new RejectFreelancerApplicationCommand(
                Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                projectId,
                applicationId), 
                cancellationToken);

        return NoContent();
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