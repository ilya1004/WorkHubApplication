using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateTextMessage;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs;

public class ChatHub(IMediator mediator, IMapper mapper) : Hub<IChatClient>
{
    public async Task SendTextMessage(SendTextMessageRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(mapper.Map<CreateTextMessageCommand>(request), cancellationToken);

        await Clients.Caller.ReceiveMessage(request.Text);
        await Clients.User(request.ReceiverId.ToString()).ReceiveMessage(request.Text);
    }

    public async Task SendFileMessage(SendFileMessageRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(mapper.Map<>());
    }
}