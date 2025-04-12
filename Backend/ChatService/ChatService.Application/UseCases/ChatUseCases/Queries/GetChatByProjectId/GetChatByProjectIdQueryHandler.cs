namespace ChatService.Application.UseCases.ChatUseCases.Queries.GetChatByProjectId;

public class GetChatByProjectIdQueryHandler(
    ILogger<GetChatByProjectIdQueryHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<GetChatByProjectIdQuery, Chat?>
{
    public async Task<Chat?> Handle(GetChatByProjectIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting chat by project ID '{ProjectId}'", request.ProjectId);
        
        var chat = await unitOfWork.ChatRepository.FirstOrDefaultAsync(c => c.ProjectId == request.ProjectId, cancellationToken);

        if (chat is null)
        {
            logger.LogWarning("Chat with project ID '{ProjectId}' not found", request.ProjectId);

            return null;
        }
        
        if (!chat.IsActive)
        {
            logger.LogWarning("Chat with project ID '{ProjectId}' is inactive", request.ProjectId);

            return null;
        }
        
        logger.LogInformation("Retrieved chat information by project ID {ChatId}", request.ProjectId);

        return chat;
    }
}