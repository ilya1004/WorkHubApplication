using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.API.Hubs;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetMessageFileById;
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
        var message = await mediator.Send(mapper.Map<CreateFileMessageCommand>(request), cancellationToken);
        
        await hubContext.Clients.User(request.ReceiverId.ToString()).ReceiveFileMessage(message);
        await hubContext.Clients.User(message.SenderId.ToString()).ReceiveFileMessage(message);
        
        logger.LogInformation("File uploaded for receiver with ID '{ReceiverId}'", request.ReceiverId);

        return Created();
    }
    
    [HttpGet]
    [Route("chat/{chatId:guid}/file/{fileId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetMessageFileById(Guid chatId, Guid fileId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMessageFileByIdQuery(chatId, fileId), cancellationToken);
    
        return File(result.Stream, result.ContentType);
    }
}
