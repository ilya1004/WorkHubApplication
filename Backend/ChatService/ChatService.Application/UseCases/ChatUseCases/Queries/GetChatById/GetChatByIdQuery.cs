namespace ChatService.Application.UseCases.ChatUseCases.Queries.GetChatById;

public sealed record GetChatByIdQuery(Guid Id) : IRequest<Chat>;