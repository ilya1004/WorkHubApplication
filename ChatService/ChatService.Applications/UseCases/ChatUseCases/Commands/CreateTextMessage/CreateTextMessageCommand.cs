namespace ChatService.Applications.UseCases.ChatUseCases.Commands.CreateTextMessage;

public sealed record CreateTextMessageCommand(
    Guid ChatId,
    Guid ReceiverId,
    string Text) : IRequest;