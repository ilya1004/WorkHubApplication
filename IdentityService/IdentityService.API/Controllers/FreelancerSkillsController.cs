using IdentityService.API.DTOs;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.CreateFreelancerSkill;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.DeleteFreelancerSkill;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.UpdateFreelancerSkill;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetAllFreelancerSkills;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetFreelancerSkillById;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FreelancerSkillsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FreelancerSkillDTO skillDTO, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateFreelancerSkillCommand(skillDTO.Name), cancellationToken);
        
        return Created();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var skill = await mediator.Send(new GetFreelancerSkillByIdQuery(id), cancellationToken);
        
        return Ok(skill);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var skills = await mediator.Send(new GetAllFreelancerSkillsQuery(pageNo, pageSize), cancellationToken);
        
        return Ok(skills);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] FreelancerSkillDTO skillDTO, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateFreelancerSkillCommand(id, skillDTO.Name), cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteFreelancerSkillCommand(id), cancellationToken);
        
        return NoContent();
    }
}
