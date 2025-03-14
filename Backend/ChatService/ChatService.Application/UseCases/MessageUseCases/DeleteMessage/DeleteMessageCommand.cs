namespace ChatService.Application.UseCases.MessageUseCases.DeleteMessage;

public sealed record DeleteMessageCommand(Guid MessageId) : IRequest;