using ChatService.API.Contracts.ChatContracts;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateTextMessage;

namespace ChatService.API.Mapping.MessageMappingProfiles;

public class CreateTextMessageRequestToCommand : Profile
{
    public CreateTextMessageRequestToCommand()
    {
        CreateMap<CreateTextMessageRequest, CreateTextMessageCommand>();
    }
}