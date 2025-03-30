using ChatService.API.Constants;
using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;
using ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetAllChats;
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
    public async Task SetChatInactive(SetChatInactiveRequest request)
    {
        await mediator.Send(mapper.Map<SetChatInactiveCommand>(request));
        
        logger.LogInformation("Chat {ChatId} set inactive", request.ChatId);
    }
    
    [Authorize]
    public async Task SendTextMessage(CreateTextMessageRequest request)
    {
        await mediator.Send(mapper.Map<CreateTextMessageCommand>(request));

        await Clients.Caller.ReceiveTextMessage(request.Text);
        await Clients.User(request.ReceiverId.ToString()).ReceiveTextMessage(request.Text);
        
        logger.LogInformation("Text message sent successfully");
    }
    
    [Authorize]
    public async Task GetChatMessages(GetChatMessagesRequest request)
    {
        var result = await mediator.Send(mapper.Map<GetChatMessagesQuery>(request));

        await Clients.Caller.ReceiveChatMessages(result);
        
        logger.LogInformation("Retrieved {MessageCount} messages", result.TotalCount);
    }

    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task GetAllChats(GetAllChatsRequest request)
    {
        var result = await mediator.Send(mapper.Map<GetAllChatsQuery>(request));

        await Clients.Caller.ReceiveAllChats(result);
        
        logger.LogInformation("Retrieved {ChatCount} chats", result.TotalCount);
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