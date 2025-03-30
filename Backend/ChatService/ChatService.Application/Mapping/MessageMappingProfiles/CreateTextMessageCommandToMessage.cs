using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;
using ChatService.Domain.Enums;

namespace ChatService.Application.Mapping.MessageMappingProfiles;

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