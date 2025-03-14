namespace ChatService.Applications.UseCases.MessageUseCases.CreateTextMessage;

public sealed record CreateTextMessageCommand(
    Guid ChatId,
    Guid ReceiverId,
    string Text) : IRequest;