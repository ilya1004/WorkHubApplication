using IdentityService.API.Contracts.EmployerIndustryContracts;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.DeleteEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetAllEmployerIndustriesQuery;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryByIdQuery;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployerIndustriesController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployerIndustryRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<CreateEmployerIndustryCommand>(request), cancellationToken);

        return Created();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var industry = await mediator.Send(new GetEmployerIndustryByIdQuery(id), cancellationToken);

        return Ok(industry);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var industries = await mediator.Send(new GetAllEmployerIndustriesQuery(), cancellationToken);
        
        return Ok(industries);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployerIndustryRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateEmployerIndustryCommand(id, request.Name), cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteEmployerIndustryCommand(id), cancellationToken);
        
        return NoContent();
    }
}

