using ChatService.Applications.UseCases.MessageUseCases.CreateTextMessage;
using ChatService.Domain.Enums;

namespace ChatService.Applications.Mapping.MessageMappingProfiles;

public class CreateTextMessageCommandToMessage : Profile
{
    public CreateTextMessageCommandToMessage()
    {
        CreateMap<CreateTextMessageCommand, Message>()
            .ForMember(dest => dest.CreatedAt, opt => 
                opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Type, opt =>
                opt.MapFrom(_ => MessageType.Text))
            .ForMember(dest => dest.SenderId, opt =>
                opt.Ignore());
    }
}