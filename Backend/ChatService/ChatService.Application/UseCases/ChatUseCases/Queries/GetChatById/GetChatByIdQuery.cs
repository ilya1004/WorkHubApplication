namespace ChatService.Application.UseCases.ChatUseCases.Queries.GetChatById;

public record GetChatByIdQuery(Guid Id) : IRequest<Chat>;