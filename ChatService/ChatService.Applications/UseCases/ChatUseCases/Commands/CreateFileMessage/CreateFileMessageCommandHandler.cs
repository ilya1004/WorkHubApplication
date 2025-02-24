namespace ChatService.Applications.UseCases.ChatUseCases.Commands.CreateFileMessage;

public class CreateFileMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IUserContext userContext) : IRequestHandler<CreateFileMessageCommand>
{
    public async Task Handle(CreateFileMessageCommand request, CancellationToken cancellationToken)
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
    }
}