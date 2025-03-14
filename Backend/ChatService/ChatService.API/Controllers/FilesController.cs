using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.API.Hubs;
using ChatService.Applications.UseCases.MessageUseCases.CreateFileMessage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController(
    IHubContext<ChatHub, IChatClient> hubContext, 
    IMediator mediator,
    IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] CreateFileMessageRequest request, 
        CancellationToken cancellationToken = default)
    {
        var fileId = await mediator.Send(mapper.Map<CreateFileMessageCommand>(request), cancellationToken);
        
        await hubContext.Clients.User(request.ReceiverId.ToString()).ReceiveFileMessage(fileId);

        return Created();
    }
}
