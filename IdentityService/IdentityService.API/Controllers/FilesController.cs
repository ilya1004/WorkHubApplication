using IdentityService.BLL.UseCases.UserUseCases.Queries.GetImageByUserId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController(IMediator mediator) : ControllerBase
{

    [HttpGet]
    [Route("by-user-id/{userId:guid}")]
    public async Task<IActionResult> GetFileByEventId(Guid userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetImageByUserIdQuery(userId), cancellationToken);

        return File(result.Stream, result.ContentType);
    }
}
