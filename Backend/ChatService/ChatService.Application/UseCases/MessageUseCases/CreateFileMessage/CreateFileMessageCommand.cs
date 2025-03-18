namespace ChatService.Application.UseCases.MessageUseCases.CreateFileMessage;

public sealed record CreateFileMessageCommand(
    Guid ChatId,
    Guid ReceiverId,
    Stream FileStream,
    string ContentType) : IRequest<Guid>;