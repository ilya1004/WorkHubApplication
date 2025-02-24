using ChatService.API.Contracts.ChatContracts;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateTextMessage;

namespace ChatService.API.Mapping.ChatMappingProfiles;

public class SendTextMessageRequestToCommand : Profile
{
    public SendTextMessageRequestToCommand()
    {
        CreateMap<SendTextMessageRequest, CreateTextMessageCommand>();
    }
}