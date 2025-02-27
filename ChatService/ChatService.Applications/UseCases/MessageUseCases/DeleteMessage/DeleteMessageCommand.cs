namespace ChatService.Applications.UseCases.ChatUseCases.Commands.DeleteMessage;

public sealed record DeleteMessageCommand(Guid MessageId) : IRequest;