using ChatService.API.Contracts.ChatContracts;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;

namespace ChatService.API.Mapping.MessageMappingProfiles;

public class CreateFileMessageRequestToCommand : Profile
{
    public CreateFileMessageRequestToCommand()
    {
        CreateMap<CreateFileMessageRequest, CreateFileMessageCommand>()
            .ForMember(dest => dest.FileStream, opt =>
                opt.MapFrom(src => src.File.OpenReadStream()))
            .ForMember(dest => dest.ContentType, opt =>
                opt.MapFrom(src => src.File.ContentType));
    }
}