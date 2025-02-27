using ChatService.Domain.Abstractions.BlobService;

namespace ChatService.Applications.UseCases.ChatUseCases.Commands.CreateFileMessage;

public class CreateFileMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IUserContext userContext,
    IBlobService blobService) : IRequestHandler<CreateFileMessageCommand, Guid>
{
    public async Task<Guid> Handle(CreateFileMessageCommand request, CancellationToken cancellationToken)
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

        var fileId = await blobService.UploadAsync(request.FileStream, request.ContentType, cancellationToken);
        message.FileId = fileId;

        await unitOfWork.MessagesRepository.InsertAsync(message, cancellationToken);

        return fileId;
    }
}