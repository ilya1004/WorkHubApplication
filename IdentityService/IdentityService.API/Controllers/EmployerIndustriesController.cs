using IdentityService.API.Constants;
using IdentityService.API.DTOs;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.DeleteEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetAllEmployerIndustries;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployerIndustriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> Create([FromBody] EmployerIndustryDTO industryDTO, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateEmployerIndustryCommand(industryDTO.Name), cancellationToken);

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
    public async Task<IActionResult> GetAll([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var industries = await mediator.Send(new GetAllEmployerIndustriesQuery(pageNo, pageSize), cancellationToken);

        return Ok(industries);
    }

    [HttpPut]
    [Route("{id:guid}")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmployerIndustryDTO industryDTO, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateEmployerIndustryCommand(id, industryDTO.Name), cancellationToken);

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
