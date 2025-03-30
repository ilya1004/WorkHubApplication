using ChatService.API.Contracts.ChatContracts;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;

namespace ChatService.API.Mapping.MessageMappingProfiles;

public class CreateTextMessageRequestToCommand : Profile
{
    public CreateTextMessageRequestToCommand()
    {
        CreateMap<CreateTextMessageRequest, CreateTextMessageCommand>();
    }
}