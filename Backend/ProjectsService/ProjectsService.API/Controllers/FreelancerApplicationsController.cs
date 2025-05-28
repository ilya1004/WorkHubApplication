using Microsoft.AspNetCore.Authorization;
using ProjectsService.API.Constants;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.AcceptFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;
using ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.RejectFreelancerApplication;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetAllFreelancerApplications;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationById;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByProjectId;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetMyFreelancerApplicationsByFilter;

namespace ProjectsService.API.Controllers;

[ApiController]
[Route("api/freelancer-applications")]
public class FreelancerApplicationsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> CreateFreelancerApplication([FromBody] CreateFreelancerApplicationRequest request, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateFreelancerApplicationCommand(request.ProjectId), cancellationToken);

        return Created();
    }

    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetFreelancerApplications([FromQuery] GetPaginatedListRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllFreelancerApplicationsQuery(request.PageNo, request.PageSize), 
            cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("{applicationId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetFreelancerApplicationById([FromRoute] Guid applicationId, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetFreelancerApplicationByIdQuery(applicationId), 
            cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("by-project/{projectId:guid}")]
    [Authorize(Policy = AuthPolicies.AdminOrEmployerPolicy)]
    public async Task<IActionResult> GetFreelancerApplicationsByProjectId([FromQuery] GetPaginatedListRequest request, 
        Guid projectId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetFreelancerApplicationsByProjectIdQuery(projectId, 
            request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("by-filter")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetFreelancerApplications([FromQuery] GetFreelancerApplicationsByFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetFreelancerApplicationsByFilterQuery>(request), cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    [Route("my-applications-filter")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> GetMyFreelancerApplications([FromQuery] GetMyFreelancerApplicationsByFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetMyFreelancerApplicationsByFilterQuery>(request), cancellationToken);

        return Ok(result);
    }
    
    [HttpPatch]
    [Route("{applicationId:guid}/accept-application/{projectId:guid}")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> AcceptApplication([FromRoute] Guid applicationId, [FromRoute] Guid projectId, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new AcceptFreelancerApplicationCommand(projectId, applicationId), cancellationToken);

        return NoContent();
    }
    
    [HttpPatch]
    [Route("{applicationId:guid}/reject-application/{projectId:guid}")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> RejectApplication([FromRoute] Guid applicationId, [FromRoute] Guid projectId, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new RejectFreelancerApplicationCommand(projectId, applicationId), cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    [Route("{applicationId:guid}")]
    [Authorize(Policy = AuthPolicies.AdminOrFreelancerPolicy)]
    public async Task<IActionResult> CancelFreelancerApplication([FromRoute] Guid applicationId, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteFreelancerApplicationCommand(applicationId), cancellationToken);

        return NoContent();
    }
}