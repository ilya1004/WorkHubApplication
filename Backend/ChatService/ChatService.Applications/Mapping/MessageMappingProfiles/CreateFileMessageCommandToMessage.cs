using ChatService.Applications.UseCases.MessageUseCases.CreateFileMessage;
using ChatService.Domain.Enums;

namespace ChatService.Applications.Mapping.MessageMappingProfiles;

public class CreateFileMessageCommandToMessage : Profile
{
    public CreateFileMessageCommandToMessage()
    {
        CreateMap<CreateFileMessageCommand, Message>()
            .ForMember(dest => dest.CreatedAt, opt => 
                opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Type, opt =>
                opt.MapFrom(_ => MessageType.File))
            .ForMember(dest => dest.SenderId, opt =>
                opt.Ignore());
    }
}