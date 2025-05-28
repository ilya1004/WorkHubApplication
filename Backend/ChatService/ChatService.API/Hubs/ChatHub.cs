using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;
using ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetChatById;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetChatByProjectId;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;
using ChatService.Application.UseCases.MessageUseCases.Commands.DeleteMessage;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs;

public class ChatHub(
    IMediator mediator, 
    IMapper mapper,
    ILogger<ChatHub> logger) : Hub<IChatClient>
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("User {UserId} connected with connection ID {ConnectionId}", Context.UserIdentifier, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("User {UserId} disconnected (Connection ID: {ConnectionId})", Context.UserIdentifier, Context.ConnectionId);
        
        if (exception is not null)
        {
            logger.LogError(exception, "Disconnection error for user {UserId}", Context.UserIdentifier);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
    
    [Authorize]
    public async Task CreateChat(CreateChatRequest request)
    {
        await mediator.Send(mapper.Map<CreateChatCommand>(request));
        
        logger.LogInformation("Chat created successfully");
    }
    
    [Authorize]
    public async Task GetChatById(Guid chatId)
    {
        var result = await mediator.Send(mapper.Map<GetChatByIdQuery>(chatId));

        await Clients.Caller.ReceiveChat(result);
        
        logger.LogInformation("Chat retrieved successfully");
    }
    
    [Authorize]
    public async Task GetChatByProjectId(Guid chatId)
    {
        var result = await mediator.Send(mapper.Map<GetChatByProjectIdQuery>(chatId));
        
        await Clients.Caller.ReceiveChat(result);
        
        logger.LogInformation("Chat retrieved successfully");
    }

    [Authorize]
    public async Task SetChatInactive(SetChatInactiveRequest request)
    {
        await mediator.Send(mapper.Map<SetChatInactiveCommand>(request));
        
        logger.LogInformation("Chat by project with ID '{ProjectId}' set inactive", request.ProjectId);
    }
    
    [Authorize]
    public async Task SendTextMessage(CreateTextMessageRequest request)
    {
        var message = await mediator.Send(mapper.Map<CreateTextMessageCommand>(request));

        await Clients.Caller.ReceiveTextMessage(message);
        await Clients.User(request.ReceiverId.ToString()).ReceiveTextMessage(message);
        
        logger.LogInformation("Text message sent successfully");
    }
    
    [Authorize]
    public async Task GetChatMessages(GetChatMessagesRequest request)
    {
        var result = await mediator.Send(mapper.Map<GetChatMessagesQuery>(request));

        await Clients.Caller.ReceiveChatMessages(result);
        
        logger.LogInformation("Retrieved {MessageCount} messages", result.TotalCount);
    }
    
    [Authorize]
    public async Task DeleteMessage(DeleteMessageRequest request)
    {
        await mediator.Send(new DeleteMessageCommand(request.MessageId));

        await Clients.Caller.MessageIsDeleted(request.MessageId);
        await Clients.User(request.ReceiverId.ToString()).MessageIsDeleted(request.MessageId);
        
        logger.LogInformation("Message with ID '{MessageId}' deleted successfully", request.MessageId);
    }
}