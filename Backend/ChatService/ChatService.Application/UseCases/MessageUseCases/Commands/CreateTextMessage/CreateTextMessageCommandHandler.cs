namespace ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;

public class CreateTextMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IMapper mapper,
    ILogger<CreateTextMessageCommandHandler> logger) : IRequestHandler<CreateTextMessageCommand, Message>
{
    public async Task<Message> Handle(CreateTextMessageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating text message in chat {ChatId} by user {UserId}", request.ChatId, userContext.GetUserId());

        var chat = await unitOfWork.ChatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            logger.LogWarning("Chat {ChatId} not found", request.ChatId);
            
            throw new NotFoundException($"Chat with ID '{request.ChatId}' not found");
        }
        
        var userId = userContext.GetUserId();

        if (chat.EmployerId != userId && chat.FreelancerId != userId)
        {
            logger.LogWarning("User {UserId} has no access to chat {ChatId}", userId, request.ChatId);
            
            throw new ForbiddenException($"You do not have access to chat with ID '{request.ChatId}'");
        }
        
        var message = mapper.Map<Message>(request);
        message.SenderId = userId;
        
        await unitOfWork.MessagesRepository.InsertAsync(message, cancellationToken);

        logger.LogInformation("Text message created successfully. Message ID: {MessageId}", message.Id);

        return message;
    }
}