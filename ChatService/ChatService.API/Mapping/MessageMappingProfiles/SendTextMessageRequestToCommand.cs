using ChatService.API.Contracts.ChatContracts;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateTextMessage;

namespace ChatService.API.Mapping.MessageMappingProfiles;

public class SendTextMessageRequestToCommand : Profile
{
    public SendTextMessageRequestToCommand()
    {
        CreateMap<SendTextMessageRequest, CreateTextMessageCommand>();
    }
}