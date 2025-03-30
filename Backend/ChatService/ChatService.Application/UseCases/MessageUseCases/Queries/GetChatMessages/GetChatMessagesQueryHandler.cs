using ChatService.Application.Models;

namespace ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;

public class GetChatMessagesQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    ILogger<GetChatMessagesQueryHandler> logger) : IRequestHandler<GetChatMessagesQuery, PaginatedResultModel<Message>>
{
    public async Task<PaginatedResultModel<Message>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting messages for chat {ChatId}. Page {PageNo}, Size {PageSize}", 
            request.ChatId, request.PageNo, request.PageSize);

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
        
        var offset = (request.PageNo - 1) * request.PageSize;
        
        var messages = await unitOfWork.MessagesRepository.GetMessagesByChatIdAsync(
            request.ChatId, offset, request.PageSize, cancellationToken);

        var messagesCount = await unitOfWork.MessagesRepository.CountAsync(
            m => m.ChatId == request.ChatId, cancellationToken);

        logger.LogInformation("Retrieved {Count} messages from chat {ChatId}", messages.Count, request.ChatId);

        return new PaginatedResultModel<Message>
        {
            Items = messages.ToList(),
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = messagesCount
        };
    }
}