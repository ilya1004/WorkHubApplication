using ChatService.Application.Models;

namespace ChatService.Application.UseCases.ChatUseCases.Queries.GetAllChats;

public class GetAllChatsQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetAllChatsQueryHandler> logger) : IRequestHandler<GetAllChatsQuery, PaginatedResultModel<Chat>>
{
    public async Task<PaginatedResultModel<Chat>> Handle(GetAllChatsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting paginated chats list. Page {PageNo}, Size {PageSize}", 
            request.PageNo, request.PageSize);
        
        var offset = (request.PageNo - 1) * request.PageSize;
        
        var chats = await unitOfWork.ChatRepository.PaginatedListAllAsync(
            offset, request.PageSize, cancellationToken);
        
        var chatsCount = await unitOfWork.ChatRepository.CountAllAsync(cancellationToken);

        logger.LogInformation("Retrieved {Count} chats out of {TotalCount}", chats.Count, chatsCount);

        return new PaginatedResultModel<Chat>
        { 
            Items = chats.ToList(),
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = chatsCount
        };
    }
}