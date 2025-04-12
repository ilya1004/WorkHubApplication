namespace ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;

public class SetChatInactiveCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<SetChatInactiveCommandHandler> logger) : IRequestHandler<SetChatInactiveCommand>
{
    public async Task Handle(SetChatInactiveCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting chat by project ID '{ProjectId}' to inactive", request.ProjectId);

        var chat = await unitOfWork.ChatRepository.FirstOrDefaultAsync(c => c.ProjectId == request.ProjectId, cancellationToken);

        if (chat is null)
        {
            logger.LogError("Chat with by project ID '{ProjectId}' not found", request.ProjectId);
        
            throw new NotFoundException($"Chat with project ID '{request.ProjectId}' not found");
        }
        
        chat.IsActive = false;
        
        await unitOfWork.ChatRepository.ReplaceAsync(chat, cancellationToken);
        
        logger.LogInformation("Chat by project ID '{ProjectId}' successfully set to inactive", request.ProjectId);
    }
}