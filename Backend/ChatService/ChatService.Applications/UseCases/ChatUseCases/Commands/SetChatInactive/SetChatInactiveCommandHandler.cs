namespace ChatService.Applications.UseCases.ChatUseCases.Commands.SetChatInactive;

public class SetChatInactiveCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<SetChatInactiveCommand>
{
    public async Task Handle(SetChatInactiveCommand request, CancellationToken cancellationToken)
    {
        var chat = await unitOfWork.ChatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            throw new NotFoundException($"Chat with ID '{request.ChatId}' not found");
        }
        
        chat.IsActive = false;
        
        await unitOfWork.ChatRepository.ReplaceAsync(chat, cancellationToken);
    }
}