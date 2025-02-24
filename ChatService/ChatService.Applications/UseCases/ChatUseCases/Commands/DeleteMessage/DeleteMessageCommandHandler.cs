namespace ChatService.Applications.UseCases.ChatUseCases.Commands.DeleteMessage;

public class DeleteMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<DeleteMessageCommand>
{
    public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await unitOfWork.MessagesRepository.GetByIdAsync(request.MessageId, cancellationToken);

        if (message is null)
        {
            throw new NotFoundException($"Message with ID '{request.MessageId}' not found");
        }
        
        var userId = userContext.GetUserId();

        if (message.SenderId != userId)
        {
            throw new ForbiddenException($"You cannot delete message with ID '{request.MessageId}' which is not yours");
        }

        await unitOfWork.MessagesRepository.DeleteAsync(request.MessageId, cancellationToken);
    }
}