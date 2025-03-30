namespace ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;

public class CreateChatCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<CreateChatCommandHandler> logger) : IRequestHandler<CreateChatCommand>
{
    public async Task Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new chat for project {ProjectId}", request.ProjectId);

        var chat = await unitOfWork.ChatRepository.FirstOrDefaultAsync(
            c => c.ProjectId == request.ProjectId, 
            cancellationToken);

        if (chat is not null)
        {
            logger.LogWarning("Chat already exists for project {ProjectId}", request.ProjectId);
            
            throw new AlreadyExistsException("Chat for this project already exists");
        }
        
        var newChat = mapper.Map<Chat>(request);
        
        await unitOfWork.ChatRepository.InsertAsync(newChat, cancellationToken);
        
        logger.LogInformation("Successfully created chat {ChatId} for project {ProjectId}", newChat.Id, request.ProjectId);
    }
}