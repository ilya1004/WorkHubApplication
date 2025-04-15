using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;
using ChatService.Domain.Enums;

namespace ChatService.Application.Mapping.MessageMappingProfiles;

public class CreateFileMessageCommandToMessage : Profile
{
    public CreateFileMessageCommandToMessage()
    {
        CreateMap<CreateFileMessageCommand, Message>()
            .ForMember(dest => dest.CreatedAt, opt => 
                opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Type, opt => 
                opt.MapFrom(_ => MessageType.File))
            .ForMember(dest => dest.Id, opt => 
                opt.MapFrom(_  => Guid.NewGuid()))
            .ForMember(dest => dest.SenderId, opt => opt.Ignore())
            .ForMember(dest => dest.Text, opt => opt.Ignore())
            .ForMember(dest => dest.FileId, opt => opt.Ignore());
    }
}