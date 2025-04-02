namespace ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;

public class SetChatInactiveCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<SetChatInactiveCommandHandler> logger) : IRequestHandler<SetChatInactiveCommand>
{
    public async Task Handle(SetChatInactiveCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting chat {ChatId} to inactive", request.ChatId);

        var chat = await unitOfWork.ChatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            logger.LogError("Chat with ID {ChatId} not found", request.ChatId);
        
            throw new NotFoundException($"Chat with ID '{request.ChatId}' not found");
        }
        
        chat.IsActive = false;
        
        await unitOfWork.ChatRepository.ReplaceAsync(chat, cancellationToken);
        
        logger.LogInformation("Chat {ChatId} successfully set to inactive", request.ChatId);
    }
}