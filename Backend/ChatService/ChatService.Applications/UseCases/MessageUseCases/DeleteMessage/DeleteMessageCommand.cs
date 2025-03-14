namespace ChatService.Applications.UseCases.MessageUseCases.DeleteMessage;

public sealed record DeleteMessageCommand(Guid MessageId) : IRequest;