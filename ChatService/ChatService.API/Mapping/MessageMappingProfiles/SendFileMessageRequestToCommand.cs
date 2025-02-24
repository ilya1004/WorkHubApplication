using ChatService.API.Contracts.ChatContracts;
using ChatService.Applications.UseCases.ChatUseCases.Commands.CreateFileMessage;

namespace ChatService.API.Mapping.MessageMappingProfiles;

public class SendFileMessageRequestToCommand : Profile
{
    public SendFileMessageRequestToCommand()
    {
        CreateMap<SendFileMessageRequest, CreateFileMessageCommand>()
            .ForMember(dest => dest.FileStream, opt =>
                opt.MapFrom(src => src.File.OpenReadStream()))
            .ForMember(dest => dest.ContentType, opt =>
                opt.MapFrom(src => src.File.ContentType));
    }
}