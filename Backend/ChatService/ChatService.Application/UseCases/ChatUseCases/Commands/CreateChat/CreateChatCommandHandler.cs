using ChatService.Application.Exceptions;

namespace ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;

public class CreateChatCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateChatCommand>
{
    public async Task Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await unitOfWork.ChatRepository.FirstOrDefaultAsync(c => c.ProjectId == request.ProjectId, cancellationToken);

        if (chat is not null)
        {
            throw new AlreadyExistsException("Chat for this already exists");
        }
        
        var newChat = mapper.Map<Chat>(request);
        
        await unitOfWork.ChatRepository.InsertAsync(newChat, cancellationToken);
    }
}