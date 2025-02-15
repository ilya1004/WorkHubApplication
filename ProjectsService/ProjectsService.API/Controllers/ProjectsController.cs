using ProjectsService.API.Contracts;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetAllProjects;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;

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
    public async Task<IActionResult> GetAllProjects([FromQuery] GetPaginatedListRequest request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllProjectsQuery(request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    [Route("/{projectId:guid}")]
    public Task<IActionResult> UpdateProjectStatus()
    {
        
    }
}