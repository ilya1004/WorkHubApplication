using Microsoft.AspNetCore.Authorization;
using ProjectsService.API.Constants;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CancelProject;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.DeleteProject;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceStatus;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetAllProjects;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFilter;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;

namespace ProjectsService.API.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var projectId = await mediator.Send(new CreateProjectCommand(request.Project, request.Lifecycle), cancellationToken);

        return Ok(new { projectId = projectId.ToString() });
    }

    [HttpGet]
    [Route("{projectId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetProjectById([FromRoute] Guid projectId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetProjectByIdQuery(projectId), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("by-filter")]
    [Authorize]
    public async Task<IActionResult> GetProjectsByFilter([FromQuery] GetProjectsByFilterRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetProjectsByFilterQuery>(request), cancellationToken);
        
        return Ok(result);
    }
    
    [HttpGet]
    [Route("my-freelancer-projects-filter")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> GetMyProjectsByFreelancerFilter([FromQuery] GetProjectsByFreelancerFilterRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetProjectsByFreelancerFilterQuery>(request), cancellationToken);
        
        return Ok(result);
    }
    
    [HttpGet]
    [Route("my-employer-projects-filter")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> GetMyProjectsByEmployerFilter([FromQuery] GetProjectsByEmployerFilterRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetProjectsByEmployerFilterQuery>(request), cancellationToken);
        
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllProjects([FromQuery] GetPaginatedListRequest request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllProjectsQuery(request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    [Route("{projectId:guid}")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> UpdateProjectData([FromRoute] Guid projectId, [FromBody] UpdateProjectRequest request, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new UpdateProjectCommand(projectId, request.Project, request.Lifecycle), cancellationToken);

        return NoContent();
    }

    [HttpPatch]
    [Route("{projectId:guid}/cancel-project")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> UpdateProjectStatus([FromRoute] Guid projectId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CancelProjectCommand(projectId), cancellationToken);

        return NoContent();
    }

    [HttpPatch]
    [Route("{projectId:guid}/request-acceptance")]
    [Authorize(Policy = AuthPolicies.FreelancerPolicy)]
    public async Task<IActionResult> UpdateAcceptanceRequest([FromRoute] Guid projectId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new UpdateAcceptanceRequestCommand(projectId), cancellationToken);

        return NoContent();
    }
    
    [HttpPatch]
    [Route("{projectId:guid}/set-acceptance-status/{status:bool}")]
    [Authorize(Policy = AuthPolicies.EmployerPolicy)]
    public async Task<IActionResult> UpdateAcceptanceStatus([FromRoute] Guid projectId, [FromRoute] bool status, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new UpdateAcceptanceStatusCommand(projectId, status), cancellationToken);

        return NoContent();
    }
    
    [HttpDelete]
    [Route("{projectId:guid}")]
    [Authorize(Policy = AuthPolicies.AdminOrEmployerPolicy)]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteProjectCommand(projectId), cancellationToken);

        return NoContent();
    }
}