using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateFileMessage;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateTextMessage;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs;

public class ChatHub(IMediator mediator, IMapper mapper) : Hub<IChatClient>
{
    public async Task SendTextMessage(SendTextMessageRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(mapper.Map<CreateTextMessageCommand>(request), cancellationToken);

        await Clients.Caller.ReceiveTextMessage(request.Text);
        await Clients.User(request.ReceiverId.ToString()).ReceiveTextMessage(request.Text);
    }

    public async Task SendFileMessage(SendFileMessageRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(mapper.Map<CreateFileMessageCommand>(request), cancellationToken);
        
        await Clients.Caller.ReceiveFileMessage(request.File);
        await Clients.User(request.ReceiverId.ToString()).ReceiveFileMessage(request.File);
    }
}