namespace ChatService.Application.UseCases.ChatUseCases.Queries.GetChatByProjectId;

public sealed record GetChatByProjectIdQuery(Guid ProjectId) : IRequest<Chat?>;