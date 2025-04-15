using ChatService.API.Contracts.ChatContracts;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;

namespace ChatService.API.Mapping.MessageMappingProfiles;

public class CreateFileMessageRequestToCommand : Profile
{
    public CreateFileMessageRequestToCommand()
    {
        CreateMap<CreateFileMessageRequest, CreateFileMessageCommand>()
            .ConstructUsing(request => new CreateFileMessageCommand(
                request.ChatId,
                request.ReceiverId,
                request.File.OpenReadStream(),
                request.File.ContentType))
            .ForAllMembers(opt => opt.Ignore());
    }
}