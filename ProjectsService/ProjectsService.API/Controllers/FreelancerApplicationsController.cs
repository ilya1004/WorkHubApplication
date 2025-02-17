using System.Security.Claims;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.AcceptFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.RejectFreelancerApplication;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetAllFreelancerApplications;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;

namespace ProjectsService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FreelancerApplicationsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateFreelancerApplication([FromBody] CreateFreelancerApplicationRequest request, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateFreelancerApplicationCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!), 
            request.ProjectId), cancellationToken);

        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetFreelancerApplications([FromQuery] GetPaginatedListRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetAllFreelancerApplicationsQuery(request.PageNo, request.PageSize), 
            cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("my-applications-filter")]
    public async Task<IActionResult> GetFreelancerApplications([FromQuery] GetFreelancerApplicationsByFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = mapper.Map<GetFreelancerApplicationsByFilterQuery>(request) with 
        { 
            FreelancerId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!) 
        };
        
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
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
    [Route("{applicationId:guid}")]
    public async Task<IActionResult> RevokeFreelancerApplication([FromRoute] Guid applicationId, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteFreelancerApplicationCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            applicationId), cancellationToken);

        return NoContent();
    }
}