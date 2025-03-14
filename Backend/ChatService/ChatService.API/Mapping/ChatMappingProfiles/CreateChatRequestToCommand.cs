using ChatService.API.Contracts.ChatContracts;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateChat;

namespace ChatService.API.Mapping.ChatMappingProfiles;

public class CreateChatRequestToCommand : Profile
{
    public CreateChatRequestToCommand()
    {
        CreateMap<CreateChatRequest, CreateChatCommand>();
    }   
}