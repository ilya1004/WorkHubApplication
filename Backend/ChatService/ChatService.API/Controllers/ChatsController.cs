using ChatService.API.Constants;
using ChatService.API.Contracts.CommonContracts;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetAllChats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers;

[ApiController]
[Route("api/chats")]
public class ChatsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllChats([FromQuery] GetPaginatedListRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllChatsQuery(request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }
}