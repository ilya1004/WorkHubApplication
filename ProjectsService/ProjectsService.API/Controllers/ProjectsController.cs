using System.Security.Claims;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.DeleteProject;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProjectStatus;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetAllProjects;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFilter;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;
using ProjectsService.Domain.Enums;

namespace ProjectsService.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(mapper.Map<CreateProjectCommand>(request), cancellationToken);

        return Created();
    }

    [HttpGet]
    [Route("{projectId:guid}")]
    public async Task<IActionResult> GetProjectById([FromRoute] Guid projectId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetProjectByIdQuery(projectId), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("by-filter")]
    public async Task<IActionResult> GetProjectsByFilter([FromQuery] GetProjectsByFilterRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetProjectsByFilterQuery>(request), cancellationToken);
        
        return Ok(result);
    }
    
    [HttpGet]
    [Route("my-projects-filter")]
    public async Task<IActionResult> GetProjectsByFreelancerFilter([FromQuery] GetProjectsByFreelancerFilterRequest request, 
        CancellationToken cancellationToken = default)
    {
        var query = mapper.Map<GetProjectsByFreelancerFilterQuery>(request) with
        {
            FreelancerId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!)
        };
        
        var result = await mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects([FromQuery] GetPaginatedListRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllProjectsQuery(request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    [Route("{projectId:guid}")]
    public async Task<IActionResult> UpdateProjectData([FromRoute] Guid projectId, [FromBody] UpdateProjectRequest request, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new UpdateProjectCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!), 
            projectId, request.Project, request.Lifecycle), 
            cancellationToken);

        return NoContent();
    }

    [HttpPut]
    [Route("{projectId:guid}/update-status")]
    public async Task<IActionResult> UpdateProjectStatus([FromRoute] Guid projectId, [FromQuery] ProjectStatus status, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new UpdateProjectStatusCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            projectId, status), 
            cancellationToken);

        return NoContent();
    }
    
    [HttpDelete]
    [Route("{projectId:guid}")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteProjectCommand(
            Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            projectId),
            cancellationToken);

        return NoContent();
    }
}