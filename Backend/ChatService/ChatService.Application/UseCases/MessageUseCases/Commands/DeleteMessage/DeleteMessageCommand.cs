namespace ChatService.Application.UseCases.MessageUseCases.Commands.DeleteMessage;

public sealed record DeleteMessageCommand(Guid MessageId) : IRequest;