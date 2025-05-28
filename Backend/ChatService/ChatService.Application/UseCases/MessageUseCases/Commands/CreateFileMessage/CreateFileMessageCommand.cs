namespace ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;

public sealed record CreateFileMessageCommand(
    Guid ChatId,
    Guid ReceiverId,
    Stream FileStream,
    string ContentType) : IRequest<Message>;