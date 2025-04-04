namespace ChatService.Application.UseCases.ChatUseCases.Queries.GetChatById;

public class GetChatByIdQueryHandler(
    ILogger<GetChatByIdQueryHandler> logger,
    IUnitOfWork unitOfWork) : IRequestHandler<GetChatByIdQuery, Chat>
{
    public async Task<Chat> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting chat by id '{ChatId}'", request.Id);
        
        var chat = await unitOfWork.ChatRepository.GetByIdAsync(request.Id, cancellationToken);

        if (chat is null)
        {
            logger.LogWarning("Chat with ID '{UserId}' not found", request.Id);
            
            throw new NotFoundException("Chat not found");
        }
        
        logger.LogInformation("Retrieved chat information by id {ChatId}", request.Id);

        return chat;
    }
}