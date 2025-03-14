using ChatService.Applications.Models;

namespace ChatService.Applications.UseCases.ChatUseCases.Queries.GetAllChats;

public class GetAllChatsQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetAllChatsQuery, PaginatedResultModel<Chat>>
{
    public async Task<PaginatedResultModel<Chat>> Handle(GetAllChatsQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;
        
        var chats = await unitOfWork.ChatRepository.PaginatedListAllAsync(
            offset, request.PageSize, cancellationToken);
        
        var chatsCount = await unitOfWork.ChatRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<Chat>
        { 
            Items = chats.ToList(),
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = chatsCount
        };
    }
}