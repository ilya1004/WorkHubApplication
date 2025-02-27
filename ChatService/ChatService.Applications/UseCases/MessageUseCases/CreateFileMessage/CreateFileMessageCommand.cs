namespace ChatService.Applications.UseCases.ChatUseCases.Commands.CreateFileMessage;

public sealed record CreateFileMessageCommand(
    Guid ChatId,
    Guid ReceiverId,
    Stream FileStream,
    string ContentType) : IRequest<Guid>;