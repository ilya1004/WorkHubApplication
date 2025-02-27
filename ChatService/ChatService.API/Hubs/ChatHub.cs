using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateFileMessage;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateTextMessage;
using ChatService.Applications.UseCases.ChatUseCases.Commands.DeleteMessage;
using ChatService.Applications.UseCases.ChatUseCases.Queries.GetAllChats;
using ChatService.Applications.UseCases.ChatUseCases.Queries.GetChatMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs;

public record TestConnectionRequest
{
    public int Qwe { get; set; }
}


// [Authorize]
public class ChatHub(IMediator mediator, IMapper mapper) : Hub<IChatClient>
{
    // public override async Task OnConnectedAsync()
    // {
    //     var userId = Context.UserIdentifier!;
    //     // await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    //     await base.OnConnectedAsync();
    // }
    //
    // public override async Task OnDisconnectedAsync(Exception? exception)
    // {
    //     var userId = Context.UserIdentifier!;
    //     // await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
    //     await base.OnDisconnectedAsync(exception);
    // }

    public async Task TestConnection(TestConnectionRequest request)
    {
        await Console.Out.WriteLineAsync("Testing connection");
        Console.WriteLine(Context.ConnectionId);
    }
    
    public async Task SendTextMessage(CreateTextMessageRequest request)
    {
        await mediator.Send(mapper.Map<CreateTextMessageCommand>(request));

        await Clients.Caller.ReceiveTextMessage(request.Text);
        await Clients.User(request.ReceiverId.ToString()).ReceiveTextMessage(request.Text);
    }

    public async Task SendFileMessage(CreateFileMessageRequest request)
    {
        var fileId = await mediator.Send(mapper.Map<CreateFileMessageCommand>(request));
        
        await Clients.Caller.ReceiveFileMessage(fileId);
        await Clients.User(request.ReceiverId.ToString()).ReceiveFileMessage(fileId);
    }

    public async Task GetChatMessages(GetChatMessagesRequest request)
    {
        var result = await mediator.Send(mapper.Map<GetChatMessagesQuery>(request));

        await Clients.Caller.ReceiveChatMessages(result);
    }

    public async Task GetAllChats(GetAllChatsRequest request)
    {
        var result = await mediator.Send(mapper.Map<GetAllChatsQuery>(request));

        await Clients.Caller.ReceiveAllChats(result);
    }
    
    public async Task DeleteMessage(DeleteMessageRequest request)
    {
        await mediator.Send(new DeleteMessageCommand(request.MessageId));

        await Clients.Caller.MessageIsDeleted(request.MessageId);
        await Clients.User(request.ReceiverId.ToString()).MessageIsDeleted(request.MessageId);
    }
}