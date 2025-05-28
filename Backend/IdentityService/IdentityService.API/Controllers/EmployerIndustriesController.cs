using IdentityService.API.Contracts.CommonContracts;
using IdentityService.API.DTOs;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.DeleteEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetAllEmployerIndustries;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryById;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/employer-industries")]
public class EmployerIndustriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> Create([FromBody] EmployerIndustryDataDto industryDataDto, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateEmployerIndustryCommand(industryDataDto.Name), cancellationToken);

        return Created();
    }

    [HttpGet]
    [Route("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var industry = await mediator.Send(new GetEmployerIndustryByIdQuery(id), cancellationToken);

        return Ok(industry);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] GetPaginatedListRequest request, CancellationToken cancellationToken = default)
    {
        var industries = await mediator.Send(new GetAllEmployerIndustriesQuery(
            request.PageNo, request.PageSize), cancellationToken);

        return Ok(industries);
    }

    [HttpPut]
    [Route("{id:guid}")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmployerIndustryDataDto industryDataDto, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateEmployerIndustryCommand(id, industryDataDto.Name), cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteEmployerIndustryCommand(id), cancellationToken);

        return NoContent();
    }
}