using ChatService.API.Constants;
using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateChat;
using ChatService.Applications.UseCases.ChatUseCases.Commands.SetChatInactive;
using ChatService.Applications.UseCases.ChatUseCases.Queries.GetAllChats;
using ChatService.Applications.UseCases.MessageUseCases.CreateTextMessage;
using ChatService.Applications.UseCases.MessageUseCases.DeleteMessage;
using ChatService.Applications.UseCases.MessageUseCases.GetChatMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs;

public class ChatHub(IMediator mediator, IMapper mapper) : Hub<IChatClient>
{
    [Authorize]
    public async Task CreateChat(CreateChatRequest request)
    {
        await mediator.Send(mapper.Map<CreateChatCommand>(request));
    }

    [Authorize]
    public async Task SetChatInactive(SetChatInactiveRequest request)
    {
        await mediator.Send(mapper.Map<SetChatInactiveCommand>(request));
    }
    
    [Authorize]
    public async Task SendTextMessage(CreateTextMessageRequest request)
    {
        await mediator.Send(mapper.Map<CreateTextMessageCommand>(request));

        await Clients.Caller.ReceiveTextMessage(request.Text);
        await Clients.User(request.ReceiverId.ToString()).ReceiveTextMessage(request.Text);
    }
    
    [Authorize]
    public async Task GetChatMessages(GetChatMessagesRequest request)
    {
        var result = await mediator.Send(mapper.Map<GetChatMessagesQuery>(request));

        await Clients.Caller.ReceiveChatMessages(result);
    }

    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task GetAllChats(GetAllChatsRequest request)
    {
        var result = await mediator.Send(mapper.Map<GetAllChatsQuery>(request));

        await Clients.Caller.ReceiveAllChats(result);
    }
    
    [Authorize]
    public async Task DeleteMessage(DeleteMessageRequest request)
    {
        await mediator.Send(new DeleteMessageCommand(request.MessageId));

        await Clients.Caller.MessageIsDeleted(request.MessageId);
        await Clients.User(request.ReceiverId.ToString()).MessageIsDeleted(request.MessageId);
    }
}