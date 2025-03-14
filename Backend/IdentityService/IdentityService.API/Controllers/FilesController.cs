using IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageById;
using IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageByUserId;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Route("by-user-id/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetFileByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetImageByUserIdQuery(userId), cancellationToken);

        return File(result.Stream, result.ContentType);
    }

    [HttpGet]
    [Route("{id:guid}")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetFileById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetImageByIdQuery(id), cancellationToken);

        return File(result.Stream, result.ContentType);
    }
}