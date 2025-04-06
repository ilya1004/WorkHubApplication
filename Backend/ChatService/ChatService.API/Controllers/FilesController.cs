using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.API.Hubs;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController(
    IHubContext<ChatHub, IChatClient> hubContext, 
    IMediator mediator,
    IMapper mapper,
    ILogger<FilesController> logger) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadFile([FromForm] CreateFileMessageRequest request, 
        CancellationToken cancellationToken = default)
    {
        var fileId = await mediator.Send(mapper.Map<CreateFileMessageCommand>(request), cancellationToken);
        
        await hubContext.Clients.User(request.ReceiverId.ToString()).ReceiveFileMessage(fileId);
        
        logger.LogInformation("File uploaded for receiver with ID '{ReceiverId}'", request.ReceiverId);

        return Created();
    }
}
