using ChatService.Domain.Abstractions.BlobService;
using ChatService.Domain.Enums;

namespace ChatService.Application.UseCases.MessageUseCases.Commands.DeleteMessage;

public class DeleteMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IBlobService blobService,
    ILogger<DeleteMessageCommandHandler> logger) : IRequestHandler<DeleteMessageCommand>
{
    public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting message {MessageId} by user {UserId}", request.MessageId, userContext.GetUserId());

        var message = await unitOfWork.MessagesRepository.GetByIdAsync(request.MessageId, cancellationToken);

        if (message is null)
        {
            logger.LogWarning("Message {MessageId} not found", request.MessageId);
            
            throw new NotFoundException($"Message with ID '{request.MessageId}' not found");
        }
        
        var userId = userContext.GetUserId();

        if (message.SenderId != userId)
        {
            logger.LogWarning("User {UserId} tried to delete message {MessageId} of user {SenderId}", 
                userId, request.MessageId, message.SenderId);
            
            throw new ForbiddenException($"You cannot delete message with ID '{request.MessageId}' which is not yours");
        }

        if (message.Type is MessageType.File && message.FileId is not null)
        {
            logger.LogInformation("Deleting file {FileId} from blob storage", message.FileId);
            
            await blobService.DeleteAsync(message.FileId.Value, cancellationToken);
        }

        await unitOfWork.MessagesRepository.DeleteAsync(request.MessageId, cancellationToken);

        logger.LogInformation("Message {MessageId} deleted successfully", request.MessageId);
    }
}