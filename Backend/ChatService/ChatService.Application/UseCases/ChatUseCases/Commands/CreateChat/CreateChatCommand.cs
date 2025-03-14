namespace ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;

public record CreateChatCommand(Guid EmployerId, Guid FreelancerId, Guid ProjectId) : IRequest;