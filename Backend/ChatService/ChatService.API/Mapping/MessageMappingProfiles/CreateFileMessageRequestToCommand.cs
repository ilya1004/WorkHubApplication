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
                request.File.ContentType));
            // .ForMember(dest => dest.ChatId, opt =>
            //     opt.MapFrom(src => src.ChatId))
            // .ForMember(dest => dest.ReceiverId, opt =>
            //     opt.MapFrom(src => src.ReceiverId))
            // .ForMember(dest => dest.FileStream, opt =>
            //     opt.MapFrom(src => src.File.OpenReadStream()))
            // .ForMember(dest => dest.ContentType, opt =>
            //     opt.MapFrom(src => src.File.ContentType));
    }
}