using ChatService.API.Contracts.ChatContracts;
using ChatService.Applications.UseCases.MessageUseCases.CreateTextMessage;

namespace ChatService.API.Mapping.MessageMappingProfiles;

public class CreateTextMessageRequestToCommand : Profile
{
    public CreateTextMessageRequestToCommand()
    {
        CreateMap<CreateTextMessageRequest, CreateTextMessageCommand>();
    }
}