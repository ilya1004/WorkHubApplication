using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.Applications.Models;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateFileMessage;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateTextMessage;
using ChatService.Applications.UseCases.ChatUseCases.Commands.DeleteMessage;
using ChatService.Applications.UseCases.ChatUseCases.Queries.GetChatMessages;
using ChatService.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs;

public class ChatHub(IMediator mediator, IMapper mapper) : Hub<IChatClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier!;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendTextMessage(SendTextMessageRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(mapper.Map<CreateTextMessageCommand>(request), cancellationToken);

        await Clients.Caller.ReceiveTextMessage(request.Text);
        await Clients.User(request.ReceiverId.ToString()).ReceiveTextMessage(request.Text);
    }

    public async Task SendFileMessage(SendFileMessageRequest request, CancellationToken cancellationToken = default)
    {
        var fileId = await mediator.Send(mapper.Map<CreateFileMessageCommand>(request), cancellationToken);
        
        await Clients.Caller.ReceiveFileMessage(fileId);
        await Clients.User(request.ReceiverId.ToString()).ReceiveFileMessage(fileId);
    }

    public async Task GetChatMessages(GetChatMessagesRequest request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(mapper.Map<GetChatMessagesQuery>(request), cancellationToken);

        await Clients.Caller.ReceiveChatMessages(result);
    }

    public async Task DeleteMessage(DeleteMessageRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteMessageCommand(request.MessageId), cancellationToken);

        await Clients.Caller.MessageIsDeleted(request.MessageId);
        await Clients.User(request.ReceiverId.ToString()).MessageIsDeleted(request.MessageId);
    }
}