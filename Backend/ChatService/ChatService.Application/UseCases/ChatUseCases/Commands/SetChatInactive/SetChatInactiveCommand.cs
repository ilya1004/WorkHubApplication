namespace ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;

public sealed record SetChatInactiveCommand(Guid ProjectId) : IRequest;