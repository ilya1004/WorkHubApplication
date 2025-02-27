using ChatService.Domain.Abstractions.BlobService;
using ChatService.Domain.Enums;

namespace ChatService.Applications.UseCases.MessageUseCases.DeleteMessage;

public class DeleteMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IBlobService blobService) : IRequestHandler<DeleteMessageCommand>
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

        if (message.Type is MessageType.File && message.FileId is not null)
        {
            await blobService.DeleteAsync(message.FileId.Value, cancellationToken);
        }

        await unitOfWork.MessagesRepository.DeleteAsync(request.MessageId, cancellationToken);
    }
}