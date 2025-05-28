using ChatService.Domain.Abstractions.BlobService;

namespace ChatService.Application.UseCases.MessageUseCases.Queries.GetMessageFileById;

public class GetMessageFileByIdQueryHandler(
    IBlobService blobService,
    ILogger<GetMessageFileByIdQueryHandler> logger,
    IUserContext userContext,
    IUnitOfWork unitOfWork) : IRequestHandler<GetMessageFileByIdQuery, FileResponse>
{
    public async Task<FileResponse> Handle(GetMessageFileByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting file by ID: {ImageId}", request.FileId);
        
        var userId = userContext.GetUserId();
        
        var chat = await unitOfWork.ChatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            logger.LogWarning("Chat with ID '{ChatId}' not found", request.ChatId);
            
            throw new NotFoundException("Chat not found");
        }

        if (chat.FreelancerId != userId && chat.EmployerId != userId)
        {
            logger.LogWarning("You do not have access to chat with ID '{ChatId}'", request.ChatId);
            
            throw new ForbiddenException($"You do not have access to this chat");
        }
        
        var result = await blobService.DownloadAsync(request.FileId, cancellationToken);
        
        logger.LogInformation("Successfully retrieved file with ID: {ImageId}", request.FileId);
        
        return result;
    }
}