namespace ChatService.Applications.UseCases.MessageUseCases.CreateTextMessage;

public class CreateTextMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IMapper mapper) : IRequestHandler<CreateTextMessageCommand>
{
    public async Task Handle(CreateTextMessageCommand request, CancellationToken cancellationToken)
    {
        var chat = await unitOfWork.ChatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            throw new NotFoundException($"Chat with ID '{request.ChatId}' not found");
        }
        
        var userId = userContext.GetUserId();

        if (chat.EmployerId != userId && chat.FreelancerId != userId)
        {
            throw new ForbiddenException($"You do not have access to chat with ID '{request.ChatId}'");
        }
        
        var message = mapper.Map<Message>(request);
        message.SenderId = userId;
        
        await unitOfWork.MessagesRepository.InsertAsync(message, cancellationToken);
    }
}