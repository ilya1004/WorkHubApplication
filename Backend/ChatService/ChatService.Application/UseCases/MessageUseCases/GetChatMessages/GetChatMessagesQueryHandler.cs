using ChatService.Application.Exceptions;
using ChatService.Application.Models;

namespace ChatService.Application.UseCases.MessageUseCases.GetChatMessages;

public class GetChatMessagesQueryHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : IRequestHandler<GetChatMessagesQuery, PaginatedResultModel<Message>>
{
    public async Task<PaginatedResultModel<Message>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
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
        
        var offset = (request.PageNo - 1) * request.PageSize;
        
        var messages = await unitOfWork.MessagesRepository.GetMessagesByChatIdAsync(
            request.ChatId, offset, request.PageSize, cancellationToken);

        var messagesCount = await unitOfWork.MessagesRepository.CountAsync(m => m.ChatId == request.ChatId, cancellationToken);

        return new PaginatedResultModel<Message>
        {
            Items = messages.ToList(),
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = messagesCount
        };
    }
}