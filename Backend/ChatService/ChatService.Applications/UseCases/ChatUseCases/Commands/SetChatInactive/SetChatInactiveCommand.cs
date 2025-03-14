namespace ChatService.Applications.UseCases.ChatUseCases.Commands.SetChatInactive;

public sealed record SetChatInactiveCommand(Guid ChatId) : IRequest;