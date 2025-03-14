namespace ChatService.Applications.UseCases.ChatUseCases.Commands.CreateChat;

public record CreateChatCommand(Guid EmployerId, Guid FreelancerId, Guid ProjectId) : IRequest;