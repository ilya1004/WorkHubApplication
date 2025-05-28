using ChatService.Domain.Abstractions.BlobService;

namespace ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;

public class CreateFileMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IUserContext userContext,
    IBlobService blobService,
    ILogger<CreateFileMessageCommandHandler> logger) : IRequestHandler<CreateFileMessageCommand, Message>
{
    public async Task<Message> Handle(CreateFileMessageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating file message in chat {ChatId} by user {UserId}", 
            request.ChatId, userContext.GetUserId());

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

        var fileId = await blobService.UploadAsync(request.FileStream, request.ContentType, cancellationToken);
        
        message.FileId = fileId;
        await unitOfWork.MessagesRepository.InsertAsync(message, cancellationToken);

        logger.LogInformation("File message created successfully. File ID: {FileId}, Message ID: {MessageId}", 
            fileId, message.Id);

        return message;
    }
}