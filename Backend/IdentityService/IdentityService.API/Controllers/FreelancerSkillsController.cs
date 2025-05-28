using IdentityService.API.Contracts.CommonContracts;
using IdentityService.API.DTOs;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.CreateFreelancerSkill;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.DeleteFreelancerSkill;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.UpdateFreelancerSkill;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetAllFreelancerSkills;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetFreelancerSkillById;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/freelancer-skills")]
public class FreelancerSkillsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> Create([FromBody] FreelancerSkillDataDto skillDataDto, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateFreelancerSkillCommand(skillDataDto.Name), cancellationToken);

        return Created();
    }

    [HttpGet]
    [Route("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var skill = await mediator.Send(new GetFreelancerSkillByIdQuery(id), cancellationToken);

        return Ok(skill);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] GetPaginatedListRequest request, CancellationToken cancellationToken = default)
    {
        var skills = await mediator.Send(new GetAllFreelancerSkillsQuery(
            request.PageNo, request.PageSize), cancellationToken);

        return Ok(skills);
    }

    [HttpPut]
    [Route("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] FreelancerSkillDataDto skillDataDto, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateFreelancerSkillCommand(id, skillDataDto.Name), cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteFreelancerSkillCommand(id), cancellationToken);

        return NoContent();
    }
}