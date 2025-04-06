namespace ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;

public sealed record CreateTextMessageCommand(
    Guid ChatId,
    Guid ReceiverId,
    string Text) : IRequest<Message>;